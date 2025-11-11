using DataverseLogic.ActivityArea;
using Microsoft.Extensions.DependencyInjection;
using Task = XrmBedrock.SharedContext.Task;

namespace DataverseRegistration.ActivityArea;

public class ValidateTask : Plugin
{
    public ValidateTask()
    {
        RegisterPluginStep<Task>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ActivityService>().ValidateTaskIsWithinBusinessHours());

        RegisterPluginStep<Task>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ActivityService>().ValidateTaskIsWithinBusinessHours())
            .AddFilteredAttributes(
                t => t.OwnerId,
                t => t.ScheduledStart)
            .AddImage(
                ImageType.PreImage,
                t => t.OwnerId,
                t => t.ScheduledStart);
    }
}