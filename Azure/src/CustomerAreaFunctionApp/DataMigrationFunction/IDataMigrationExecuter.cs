using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

public interface IDataMigrationExecuter
{
    Task MigrateChunkListAsync(
        string connectionString,
        string environmentIdentifier,
        IList<DataRow[]> chunkList,
        JobInstance jobInstance,
        Action checkForStopOnTimeDelegate,
        CancellationToken cancellationToken);
}