using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Task = XrmBedrock.SharedContext.Task;

namespace DataverseLogic.ActivityArea;

public class ActivityService : IActivityService
{
    private readonly IPluginExecutionContext context;
    private readonly ILogger logger;

    public ActivityService(
        IPluginExecutionContext context,
        ILogger logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public void DemoValidateTask()
    {
        // Generally do not bloat your code with logging - this is just to demonstrate how to use it.
        logger.LogInformation("Validating task.");

        // Get the values of interest from the target entity and missing values from the preimage. For a Create the preimage will be empty but the GetTargetMergedWithPreImage is robust to that
        var task = context.GetTargetMergedWithPreImage<Task>();

        // if the owner of the task is not someone else than the current user, then we do not validate the scheduled start
        if (task.OwnerId == null || task.OwnerId.Id == context.UserId)
            return;

        if (!IsWithinBusinessHours(task.ScheduledStart))
            throw new InvalidPluginExecutionException("Task is not scheduled within business hours.");
    }

    private static bool IsWithinBusinessHours(DateTime? scheduledStart)
    {
        if (scheduledStart == null)
            return false;

        var hour = scheduledStart.Value.Hour;
        return hour >= 8 && hour < 17;
    }
}