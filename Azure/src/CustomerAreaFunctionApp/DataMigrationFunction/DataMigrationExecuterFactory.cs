namespace CustomerAreaFunctionApp.DataMigrationFunction;

public class DataMigrationExecuterFactory : IDataMigrationExecuterFactory
{
    private readonly IServiceProvider provider;

    public DataMigrationExecuterFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public IDataMigrationExecuter CreateExecuter()
    {
        var executer = provider.GetService(typeof(IDataMigrationExecuter));
        if (executer is null)
            throw new ArgumentException(typeof(IDataMigrationExecuter).FullName, $"Cannot resolve service {typeof(IDataMigrationExecuter).FullName}");
        return (IDataMigrationExecuter)executer;
    }
}