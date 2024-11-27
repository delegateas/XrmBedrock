using System.Data;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

public static class StaticExtentions
{
    public static T? GetFieldValue<T>(this DataRow row, string columnName)
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

    public static DateTime? GetSafeDateTime(this DataRow row, string columnName)
    {
        var dt = row.GetFieldValue<DateTime?>(columnName);

        if (!dt.HasValue)
        {
            return null;
        }

        if (dt.Value.Date == DateTime.MaxValue.Date)
        {
            return null;
        }

        return dt;
    }
}