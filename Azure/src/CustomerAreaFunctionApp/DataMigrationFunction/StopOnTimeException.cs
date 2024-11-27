namespace CustomerAreaFunctionApp.DataMigrationFunction;

public class StopOnTimeException : Exception
{
    public StopOnTimeException()
    {
    }

    public StopOnTimeException(string message)
        : base(message)
    {
    }

    public StopOnTimeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}