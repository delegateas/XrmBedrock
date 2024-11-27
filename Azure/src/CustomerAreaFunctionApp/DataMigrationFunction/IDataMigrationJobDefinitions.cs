namespace CustomerAreaFunctionApp.DataMigrationFunction;

public interface IDataMigrationJobDefinitions
{
    DataMigrationJob GetByJobType(string jobType);

    void SetEnvironmentIdentifier(string environmentIdentifier);
}