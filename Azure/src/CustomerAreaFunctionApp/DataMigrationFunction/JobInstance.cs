namespace CustomerAreaFunctionApp.DataMigrationFunction;

public sealed class JobInstance
{
    public int JobId { get; set; }

    public string JobType { get; set; }

    public string Status { get; set; }

    public bool ErrorHasOccurred { get; set; }

    public JobInstance(int jobId, string jobType, string status, bool errorHasOccurred)
    {
        this.JobId = jobId;
        this.JobType = jobType;
        this.Status = status;
        this.ErrorHasOccurred = errorHasOccurred;
    }
}