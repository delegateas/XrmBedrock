namespace CustomerAreaFunctionApp.DataMigrationFunction;

public static class JobStatus
{
    public const string Ready = "Ready";
    public const string InProgress = "In progress";
    public const string StoppedReadyToContinue = "Stopped, ready to continue";
    public const string Completed = "Completed";
    public const string FailedToStart = "Failed to start";
}