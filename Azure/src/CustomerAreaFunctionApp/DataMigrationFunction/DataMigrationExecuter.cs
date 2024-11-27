using DataverseService.UtilityArea.DataMigration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Concurrent;
using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

public class DataMigrationExecuter : IDataMigrationExecuter, IDisposable
{
    private readonly IDataMigrationJobDefinitions dataMigrationJobDefinitions;
    private readonly IDataverseMigrateGenericService dataverseMigrateGenericService;
    private readonly ILogger<DataMigrationExecuter> logger;
    private readonly Guid tag = Guid.NewGuid();
    private BookKeeping? bookKeeping;

    public DataMigrationExecuter(
        IDataMigrationJobDefinitions dataMigrationJobDefinitions,
        IDataverseMigrateGenericService dataverseMigrateGenericService,
        ILogger<DataMigrationExecuter> logger)
    {
        this.dataMigrationJobDefinitions = dataMigrationJobDefinitions;
        this.dataverseMigrateGenericService = dataverseMigrateGenericService;
        this.logger = logger;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            bookKeeping?.Dispose();
        }
    }

    public async Task MigrateChunkListAsync(
        string connectionString,
        string environmentIdentifier,
        IList<DataRow[]> chunkList,
        JobInstance jobInstance,
        Action checkForStopOnTimeDelegate,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"DataMigrationExecuter {tag} started");
        if (chunkList is null)
            throw new InvalidDataException("ChunkList is null");
        if (jobInstance is null)
            throw new InvalidDataException("JobInstance is null");

        bookKeeping = new BookKeeping(environmentIdentifier, connectionString, logger, cancellationToken);
        var dataMigrationJob = dataMigrationJobDefinitions.GetByJobType(jobInstance.JobType);
        if (dataMigrationJob == null)
        {
            bookKeeping.RegisterFatalJob(jobInstance, $"Unknown job type {jobInstance.JobType}");
            return;
        }

        try
        {
            foreach (var chunk in chunkList)
            {
                await MigrateChunkAsync(chunk, dataMigrationJob, jobInstance).ConfigureAwait(false);
                if (checkForStopOnTimeDelegate != null)
                    checkForStopOnTimeDelegate();
            }
        }
        catch (StopOnTimeException)
        {
            logger.LogInformation($"DataMigrationExecuter {tag} was stopped on time");
        }

        logger.LogInformation($"DataMigrationExecuter {tag} completed");
    }

    private OrganizationRequest? WrapToCatchAndHandleExceptions(DataRow row, Func<DataRow, OrganizationRequest> mapper, ConcurrentBag<MappingError> faildBag)
    {
        if (bookKeeping is null)
            throw new InvalidDataException("BookKeeping is null");

        try
        {
            return mapper(row) ?? throw new InvalidDataException("Mapper does not produce a request");
        }
        catch (OperationCanceledException oce)
        {
            logger.LogError(oce, "MigrateData execution of element operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            // we handle the bookkeeping afterwards as the SQL connection does not support the concurrent usage
            faildBag.Add(new MappingError(row, ex));
            return null;
        }
    }

    private async Task MigrateChunkAsync(DataRow[] chunk, DataMigrationJob dataMigrationJob, JobInstance jobInstance)
    {
        if (bookKeeping is null)
            throw new InvalidDataException("BookKeeping is null");

        try
        {
            var faildBag = new ConcurrentBag<MappingError>();
            var taskList = new List<Task<OrganizationRequest?>>();
            foreach (var row in chunk)
            {
                var task = Task.Run(() => WrapToCatchAndHandleExceptions(row, dataMigrationJob.Mapper, faildBag));
                taskList.Add(task);
            }

            var dirtyRequestList = await Task.WhenAll(taskList).ConfigureAwait(false);
            HandledFaildBag(faildBag, dataMigrationJob, jobInstance);
            var (cleanedRequestList, matchingChunk) = FilterDirtyRequests(dirtyRequestList, chunk);
            var responseOfExecuteMultiple = await ExecuteMultipleRequestAsync(cleanedRequestList).ConfigureAwait(false);
            if (responseOfExecuteMultiple is ExecuteMultipleResponse emr)
            {
                foreach (var response in emr.Responses)
                {
                    if (response.Fault != null)
                    {
                        bookKeeping.RegisterFailedElement(dataMigrationJob, matchingChunk[response.RequestIndex], response.Fault);
                        jobInstance.ErrorHasOccurred = true;
                    }
                    else
                    {
                        bookKeeping.RegisterSuccessfulElement(dataMigrationJob, matchingChunk[response.RequestIndex]);
                    }
                }
            }
        }
        catch (OperationCanceledException oce)
        {
            logger.LogError(oce, "MigrateData execution of element operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            bookKeeping?.RegisterFailedElements(dataMigrationJob, chunk, ex);
            throw;
        }
    }

    private void HandledFaildBag(ConcurrentBag<MappingError> faildBag, DataMigrationJob dataMigrationJob, JobInstance jobInstance)
    {
        if (bookKeeping is null)
            throw new InvalidDataException("BookKeeping is null");
        if (faildBag.IsEmpty)
            return;
        jobInstance.ErrorHasOccurred = true;
        foreach (var (row, ex) in faildBag)
            bookKeeping.RegisterFailedElement(dataMigrationJob, row, ex);
    }

    private Task<OrganizationResponse> ExecuteMultipleRequestAsync(List<OrganizationRequest> cleanedRequestList)
    {
        var organizationRequestCollection = new OrganizationRequestCollection();
        organizationRequestCollection.AddRange(cleanedRequestList);
        var executeMultipleRequest = new ExecuteMultipleRequest
        {
            Requests = organizationRequestCollection,
            Settings = new ExecuteMultipleSettings
            {
                ContinueOnError = true,
                ReturnResponses = true,
            },
        };
        return dataverseMigrateGenericService.ExecuteUnawaitedAsync(executeMultipleRequest);
    }

    private static (List<OrganizationRequest> CleanedRequestList, List<DataRow> MatchingChunk) FilterDirtyRequests(OrganizationRequest?[] dirtyRequestList, DataRow[] chunk)
    {
        var cleanedRequestList = new List<OrganizationRequest>();
        var matchingChunk = new List<DataRow>();
        for (var i = 0; i < dirtyRequestList.Length; i++)
        {
            var request = dirtyRequestList[i];
            if (request != null)
            {
                cleanedRequestList.Add(request);
                matchingChunk.Add(chunk[i]);
            }
        }

        return (cleanedRequestList, matchingChunk);
    }
}