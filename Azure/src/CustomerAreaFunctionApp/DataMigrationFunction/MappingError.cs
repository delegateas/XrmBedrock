using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

internal sealed record MappingError(DataRow Row, Exception Exception);