using DataverseService.UtilityArea.DataMigration;
using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

#pragma warning disable CA1812 // Stylecop doesn't recognize that this class is used in the DI container

internal sealed partial class DataMigrationJobDefinitions : IDataMigrationJobDefinitions
#pragma warning restore CA1812 // Stylecop doesn't recognize that this class is used in the DI container
{
    private readonly IDataverseMigrateGenericService dataverseMigrateGenericService;
    private readonly Dictionary<string, DataMigrationJob> jobDictionary;
    private string? dataMigrationTestMarking;

    public DataMigrationJobDefinitions(IDataverseMigrateGenericService dataverseMigrateGenericService)
    {
        this.dataverseMigrateGenericService = dataverseMigrateGenericService;
        jobDictionary = RegisterJobs();
    }

    public void SetEnvironmentIdentifier(string environmentIdentifier)
    {
        this.dataMigrationTestMarking = environmentIdentifier == "Dev" ? "DataMigDoNotUse " : string.Empty;
    }

    private Dictionary<string, DataMigrationJob> RegisterJobs()
    {
        // Please keep sorted by JobType to maintain readability
        var jobs = new DataMigrationJob[]
        {
        new("Account", "AccountVw", "sourceId", "int", AccountMapper),
        new("AccountHiearchy", "AccountHiearchyVw", "sourceId", "int", AccountHierarchyMapper),
        new("Contact", "ContactVw", "sourceId", "int", ContactMapper),
        };
        var dict = new Dictionary<string, DataMigrationJob>(StringComparer.OrdinalIgnoreCase);
        jobs.ToList().ForEach(job => dict.Add(job.JobType ?? throw new InvalidDataException("JobType cannot be empty"), job));
        return dict;
    }

    public DataMigrationJob GetByJobType(string jobType)
    {
        if (jobDictionary.TryGetValue(jobType, out var job))
            return job;
        throw new NotSupportedException($"JobType {jobType} is not supported");
    }

    private static T? GetFieldValue<T>(DataRow row, string columnName)
    {
        try
        {
            return row.Field<T>(columnName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting field value for column {columnName}", ex);
        }
    }
}