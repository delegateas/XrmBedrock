using Microsoft.Xrm.Sdk;
using SharedDomain;
using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

public record DataMigrationJob(string JobType, string ViewName, string ElementIdName, string ElementIdType, Func<DataRow, OrganizationRequest> Mapper)
{
    public string? GetElementIdValueAsString(DataRow row)
    {
        switch (ElementIdType.ToLowerSolutionDefault())
        {
            case "int":
                return row.Field<int>(ElementIdName).ToStringSolutionDefault();
            case "guid":
                return row.Field<Guid>(ElementIdName).ToString();
            case "string":
                return row.Field<string>(ElementIdName);
            default:
                throw new NotSupportedException($"ElementIdType {ElementIdType} is not supported");
        }
    }
}