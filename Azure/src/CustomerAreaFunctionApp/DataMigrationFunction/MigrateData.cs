using DataverseService.UtilityArea.DataMigration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

public class MigrateData : IDisposable
{
    private const int AllowedExecutionTimeInMinutes = 28;
    private readonly int chunkSize;
    private readonly int limitJobToTop;
    private readonly int parallelExecuters;
    private readonly ILogger<MigrateData> logger;
    private readonly IDataverseMigrateGenericService dataverseMigrateGenericService;
    private readonly IDataMigrationJobDefinitions dataMigrationJobDefinitions;
    private readonly IDataMigrationExecuterFactory dataMigrationExecuterFactory;
    private readonly string connectionString;
    private readonly string? runnerScope;
    private readonly string environmentIdentifier;
    private readonly DateTime startTimestamp = DateTime.UtcNow;
    private BookKeepingExtended? bookKeeping;
    private CancellationToken cancellationToken;

    public MigrateData(
        IDataMigrationExecuterFactory dataMigrationExecuterFactory,
        IDataMigrationJobDefinitions dataMigrationJobDefinitions,
        IDataverseMigrateGenericService dataverseMigrateGenericService,
        IConfiguration configuration,
        ILogger<MigrateData> logger,
        IMemoryCache cache)
    {
        this.dataMigrationExecuterFactory = dataMigrationExecuterFactory;
        this.dataMigrationJobDefinitions = dataMigrationJobDefinitions;
        this.dataverseMigrateGenericService = dataverseMigrateGenericService;
        this.logger = logger;
        connectionString = configuration.GetValue<string>("DMAzureSqlConnectionString") ?? throw new InvalidDataException("Connection string 'DMAzureSqlConnectionString' is missing from configuration");
        runnerScope = configuration.GetValue<string>("RunnerScope");
        chunkSize = configuration.GetValue<int?>("ChunkSize") ?? 10;
        limitJobToTop = configuration.GetValue<int?>("LimitJobToTop") ?? 1000000;
        parallelExecuters = configuration.GetValue<int?>("ParallelExecuters") ?? 5;
        environmentIdentifier = GetEnvironmentIdentifier(configuration);
        this.dataMigrationJobDefinitions.SetEnvironmentIdentifier(environmentIdentifier);
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

    private static string GetEnvironmentIdentifier(IConfiguration configuration)
    {
        var environmentIndicatorFromConfig = configuration.GetValue<string>("DataverseConnectionstring")
            ?? configuration.GetValue<string>("DataverseUrl")
            ?? throw new InvalidDataException("Unable to find environmentIdentifier, neither DataverseConnectionstring nor DataverseUrl found in configuration");
        if (environmentIndicatorFromConfig.Contains("xxxx-udv.crm4.dynamics.com", StringComparison.InvariantCultureIgnoreCase))
            return "Dev";
        if (environmentIndicatorFromConfig.Contains("xxxx-test.crm4.dynamics.com", StringComparison.InvariantCultureIgnoreCase))
            return "Test";
        if (environmentIndicatorFromConfig.Contains("xxxx-uat.crm4.dynamics.com", StringComparison.InvariantCultureIgnoreCase))
            return "Uat";
        if (environmentIndicatorFromConfig.Contains("xxxx.crm4.dynamics.com", StringComparison.InvariantCultureIgnoreCase))
            return "Prod";
        throw new InvalidDataException("Unable to find environmentIdentifier, Unknown environment");
    }

    [Function("MigrateData")]
#if DEBUG
    // Running on startup locally/Debug and scheduled to "almost never" (well and at midnight of the 29th of february...)
    public async Task Run([TimerTrigger("0 0 0 29 2 *", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationTokenArg)
#else
    // Running every 30 minutes in azure/Release
    public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = false)] TimerInfo myTimer, CancellationToken cancellationTokenArg)
#endif
    {
        logger.LogInformation($"MigrateData Running for RunnerScope {runnerScope ?? "(default scope)"} with ChunkSize {chunkSize}, ParallelExecuters {parallelExecuters}, LimitJobToTop {limitJobToTop}");
        JobInstance? jobInstance = null;
        try
        {
            cancellationToken = cancellationTokenArg;
            dataverseMigrateGenericService.SetCancellationToken(cancellationToken);
            bookKeeping = new BookKeepingExtended(environmentIdentifier, runnerScope, connectionString, limitJobToTop, logger, cancellationToken);
            logger.LogInformation("DB Connection established");
            if (bookKeeping.IsAJobAlreadyInProgress())
            {
                logger.LogInformation($"A job is already in progress for RunnerScope {runnerScope ?? "(default scope)"}, skipping this run");
                return;
            }

            while (true)
            {
                try
                {
                    jobInstance = bookKeeping.GetNextJobInstance();
                    if (jobInstance == null)
                        break;
                    await PerformMigrationAsync(jobInstance).ConfigureAwait(false);
                    CheckForStopOnTime();
                }
                catch (OperationCanceledException oce)
                {
                    logger.LogError(oce, "MigrateData execution was cancelled and sets jobstatus to be ready to continue");
                    if (jobInstance != null)
                        bookKeeping.RegisterPausedJob(jobInstance);
                    break;
                }
                catch (StopOnTimeException sote)
                {
                    logger.LogError(sote, "MigrateData execution was stopped on time and sets jobstatus to be ready to continue");
                    if (jobInstance != null)
                        bookKeeping.RegisterPausedJob(jobInstance);
                    break;
                }
            }
        }
        finally
        {
            logger.LogInformation($"MigrateData Done for RunnerScope {runnerScope ?? "(default scope)"}");
            bookKeeping?.Dispose();
        }
    }

    private void CheckForStopOnTime()
    {
        if (DateTime.UtcNow - startTimestamp > TimeSpan.FromMinutes(AllowedExecutionTimeInMinutes))
            throw new StopOnTimeException("StopOnTimeException");
    }

    private async Task PerformMigrationAsync(JobInstance jobInstance)
    {
        if (bookKeeping is null)
            throw new InvalidDataException("BookKeeping is null");
        logger.LogInformation($"Handling job {jobInstance.JobType}");
        var dataMigrationJob = dataMigrationJobDefinitions.GetByJobType(jobInstance.JobType);
        if (dataMigrationJob == null)
        {
            bookKeeping.RegisterFatalJob(jobInstance, $"Unknown job type {jobInstance.JobType}");
            return;
        }

        bookKeeping.RegisterJobInProgress(jobInstance);
        DataTable? elementsToMigrate = null;
        try
        {
            try
            {
                elementsToMigrate = bookKeeping.GetElementsToMigrate(dataMigrationJob);
                if (elementsToMigrate == null)
                    throw new InvalidDataException("Data table with elements to retrieve was null");
            }
            catch (Exception ex)
            {
                bookKeeping.RegisterFatalJob(jobInstance, $"Failed to retrieve elements to migrate for job type {jobInstance.JobType}: {ex}");
                return;
            }

            logger.LogInformation($"Retrieved {elementsToMigrate.Rows.Count} from source {dataMigrationJob.ViewName}");
            if (chunkSize > 1)
                await PerformMigrationInParallelAsync(jobInstance, elementsToMigrate).ConfigureAwait(false);
            else
                await PerformMigrationOneByOneAsync(jobInstance, dataMigrationJob, elementsToMigrate).ConfigureAwait(false);
            bookKeeping.RegisterCompletedJob(jobInstance);
        }
        finally
        {
            if (elementsToMigrate != null)
                elementsToMigrate.Dispose();
        }
    }

    private async Task PerformMigrationOneByOneAsync(JobInstance jobInstance, DataMigrationJob dataMigrationJob, DataTable elementsToMigrate)
    {
        if (bookKeeping is null)
            throw new InvalidDataException("BookKeeping is null");

        foreach (DataRow row in elementsToMigrate.Rows)
        {
            try
            {
                var request = dataMigrationJob.Mapper(row) ?? throw new InvalidDataException("Mapper does not produce a request");
                await dataverseMigrateGenericService.ExecuteUnawaitedAsync(request).ConfigureAwait(false);
                bookKeeping.RegisterSuccessfulElement(dataMigrationJob, row);
            }
            catch (OperationCanceledException oce)
            {
                logger.LogError(oce, "MigrateData execution of element operation was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                bookKeeping.RegisterFailedElement(dataMigrationJob, row, ex);
                jobInstance.ErrorHasOccurred = true;
            }
            finally
            {
                CheckForStopOnTime();
            }
        }
    }

    private Task PerformMigrationInParallelAsync(JobInstance jobInstance, DataTable elementsToMigrate)
    {
        var i = 0;
        var j = 0;
        var chunk = new DataRow[chunkSize];
        var listOfChunklists = new List<List<DataRow[]>>();
        for (var k = 0; k < parallelExecuters; k++)
            listOfChunklists.Add(new List<DataRow[]>());
        foreach (DataRow row in elementsToMigrate.Rows)
        {
            chunk[i] = row;
            i++;
            if (i == chunkSize)
            {
                listOfChunklists[j].Add(chunk);
                i = 0;
                chunk = new DataRow[chunkSize];
                j = j == parallelExecuters - 1 ? 0 : j + 1;
            }
        }

        // remember remaining piece of a chunk
        if (i > 0)
            listOfChunklists[j].Add(chunk.AsEnumerable().Take(i).ToArray());

        var taskList = new List<Task>();
        listOfChunklists.ForEach(cl => taskList.Add(dataMigrationExecuterFactory.CreateExecuter().MigrateChunkListAsync(connectionString, environmentIdentifier, cl, jobInstance, CheckForStopOnTime, cancellationToken)));
        return Task.WhenAll(taskList);
    }
}