namespace CustomerAreaFunctionApp.DataMigrationFunction;

public interface IDataMigrationExecuterFactory
{
    IDataMigrationExecuter CreateExecuter();
}