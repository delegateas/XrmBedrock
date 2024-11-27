using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SharedDomain;
using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

internal sealed class BookKeepingExtended : BookKeeping
{
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
    private readonly int limitJobToTop;
    private readonly string? runnerScope;

    public BookKeepingExtended(string environmentIdentifier, string? runnerScope, string connectionString, int limitJobToTop, ILogger<MigrateData> logger, CancellationToken cancellationToken)
        : base(environmentIdentifier, connectionString, logger, cancellationToken)
    {
        this.limitJobToTop = limitJobToTop;
        this.runnerScope = runnerScope;
    }

    private string RunnerScopeRestrictionString() => runnerScope == null ? "IS NULL" : $"= '{runnerScope}'";

    public bool IsAJobAlreadyInProgress()
    {
        var sql = $"SELECT * FROM {GetJobTableName()} j WHERE j.DmStatus IN ('{JobStatus.InProgress}') AND j.DmRunnerScope {RunnerScopeRestrictionString()}";
        using var command = new SqlCommand(sql, GetDbConnection());
        using var query = command.ExecuteReader();
        return query.HasRows;
    }

    public JobInstance? GetNextJobInstance()
    {
        var sql = $"SELECT TOP 1 * FROM {GetJobTableName()} j WHERE j.DmStatus IN ('{JobStatus.Ready}','{JobStatus.StoppedReadyToContinue}') AND j.DmRunnerScope {RunnerScopeRestrictionString()} ORDER BY j.DmPriority";
        using var command = new SqlCommand(sql, GetDbConnection());
        using var adapter = new SqlDataAdapter(command);
        using var dataTable = new DataTable();
        adapter.Fill(dataTable);
        if (dataTable.Rows.Count == 0)
            return null;
        var jobType = dataTable.Rows[0].Field<string>("DmJobType") ?? throw new InvalidDataException("JobType is null");
        var status = dataTable.Rows[0].Field<string>("DmStatus") ?? throw new InvalidDataException("Status is null");
        return new JobInstance(dataTable.Rows[0].Field<int>("DmId"), jobType, status, dataTable.Rows[0].Field<bool?>("DmErrorsOccurred") ?? false);
    }

    public DataTable GetElementsToMigrate(DataMigrationJob dataMigrationJob)
    {
        var sql = @$"
SELECT top {limitJobToTop.ToStringLfDefault()} * FROM {dataMigrationJob.ViewName} d
LEFT JOIN {GetElementStatusTableName()} s ON d.{dataMigrationJob.ElementIdName} = s.DmElementId and s.DmJobType = '{dataMigrationJob.JobType}'
WHERE s.DmStatus IS NULL OR s.DmStatus = '{ElementStatus.Retry}'
ORDER BY s.DmStatus, d.{dataMigrationJob.ElementIdName}";

        using var command = new SqlCommand(sql, GetDbConnection());
        using var adapter = new SqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
}