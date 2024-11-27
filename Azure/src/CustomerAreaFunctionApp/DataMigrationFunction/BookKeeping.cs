using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedDomain;
using System.Data;
using System.Diagnostics;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

internal class BookKeeping : IDisposable
{
    private readonly string environmentIdentifier;
    private readonly string connectionString;
    private readonly ILogger logger;
    private SqlConnection? dbConnection;

    protected string EnvironmentIdentifier => environmentIdentifier;

    protected string ConnectionString => connectionString;

    protected ILogger Logger => logger;

    protected SqlConnection? GetDbConnection() => dbConnection;

    protected void SetDbConnection(SqlConnection? value) => dbConnection = value;

    public BookKeeping(string environmentIdentifier, string connectionString, ILogger logger, CancellationToken cancellationToken)
    {
        this.environmentIdentifier = environmentIdentifier;
        this.connectionString = connectionString;
        this.logger = logger;
        SetDbConnection(new SqlConnection(connectionString));
        GetDbConnection()?.OpenAsync(cancellationToken).GetAwaiter().GetResult();
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
            GetDbConnection()?.Close();
            GetDbConnection()?.Dispose();
        }
    }

    public void RegisterSuccessfulElement(DataMigrationJob dataMigrationJob, DataRow row)
    {
        var dmId = row.Field<int?>("DmId");
        var sql = dmId != null
            ? $"UPDATE {GetElementStatusTableName()} SET DmStatus = '{ElementStatus.Success}', DmMessage = NULL WHERE DmId = {dmId.Value.ToStringSolutionDefault()}"
            : $"INSERT INTO {GetElementStatusTableName()} (DmJobType, DMElementId, DmStatus, DmMessage) VALUES ('{dataMigrationJob.JobType}', '{dataMigrationJob.GetElementIdValueAsString(row)}', '{ElementStatus.Success}', NULL)";
        ExecuteNonQuery(sql);
    }

    public void RegisterFailedElements(DataMigrationJob dataMigrationJob, DataRow[] rows, Exception ex)
    {
        if (rows is null)
            return;
        foreach (var row in rows)
            RegisterFailedElement(dataMigrationJob, row, ex);
    }

    public void RegisterFailedElement(DataMigrationJob dataMigrationJob, DataRow row, Exception ex)
    {
        var exStr = ex switch
        {
            InvalidDataException ide => ide.Message?.Replace("'", "''", StringComparison.InvariantCultureIgnoreCase),
            _ => ex.ToInvariantString().Replace("'", "''", StringComparison.InvariantCultureIgnoreCase),
        };
        RegisterFailedElement(dataMigrationJob, row, exStr);
    }

    public void RegisterFailedElement(DataMigrationJob dataMigrationJob, DataRow row, OrganizationServiceFault fault)
    {
        var faultStr = fault?.Message?.Replace("'", "''", StringComparison.InvariantCultureIgnoreCase) ?? "Unknown fault";
        RegisterFailedElement(dataMigrationJob, row, faultStr);
    }

    private void RegisterFailedElement(DataMigrationJob dataMigrationJob, DataRow row, string? errorMsg)
    {
        var dmId = row.Field<int?>("DmId");
        var sql = dmId != null
            ? $"UPDATE {GetElementStatusTableName()} SET DmStatus = '{ElementStatus.Failure}', DmMessage = '{errorMsg}' WHERE DmId = {dmId.Value.ToStringSolutionDefault()}"
            : $"INSERT INTO {GetElementStatusTableName()} (DmJobType, DMElementId, DmStatus, DmMessage) VALUES ('{dataMigrationJob.JobType}', '{dataMigrationJob.GetElementIdValueAsString(row)}', '{ElementStatus.Failure}', '{errorMsg}')";
        ExecuteNonQuery(sql);
    }

    public void RegisterCompletedJob(JobInstance jobInstance) => RegisterJobStatus(jobInstance, JobStatus.Completed, null);

    public void RegisterFatalJob(JobInstance jobInstance, string message)
    {
        Logger.LogError(message);
        RegisterJobStatus(jobInstance, JobStatus.FailedToStart, message);
    }

    public void RegisterJobInProgress(JobInstance jobInstance) => RegisterJobStatus(jobInstance, JobStatus.InProgress, null);

    public void RegisterPausedJob(JobInstance jobInstance)
    {
        if (GetDbConnection()?.State != ConnectionState.Open)
            GetDbConnection()?.Open();
        RegisterJobStatus(jobInstance, JobStatus.StoppedReadyToContinue, null);
    }

    private void RegisterJobStatus(JobInstance jobInstance, string jobStatus, string? message)
    {
        var sql = $"UPDATE {GetJobTableName()} SET DmStatus = '{jobStatus}', DmMessage = {(message == null ? "NULL" : "'" + message + "'")}, DmErrorsOccurred = {(jobInstance.ErrorHasOccurred ? "1" : "0")} WHERE DmId = {jobInstance.JobId.ToStringSolutionDefault()}";
        ExecuteNonQuery(sql);
    }

    protected string GetJobTableName() => $"Dm{EnvironmentIdentifier}JobTbl";

    protected string GetElementStatusTableName() => $"Dm{EnvironmentIdentifier}ElementStatusTbl";

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities

    private void ExecuteNonQuery(string sql)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            using var command = new SqlCommand(sql, GetDbConnection());

            // There seems to be a bug in ExecuteNonQueryAsync, so we use ExecuteNonQuery instead
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to execute sql: {sql}. Ran for {stopWatch.ElapsedMilliseconds} ms. Details: {ex}");
            throw;
        }
        finally
        {
            stopWatch.Stop();
        }
    }
}