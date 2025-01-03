using DataverseLogic.ActivityArea;
using Microsoft.Extensions.DependencyInjection;
using Task = XrmBedrock.SharedContext.Task;

namespace Dataverse.Plugins.ActivityArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// This plugin is responsible for validating that a task assigned to someone else is scheduled within business hours.
/// </summary>
public class DemoValidateTask : Plugin
{
    public DemoValidateTask()
        : base(typeof(DemoValidateTask))
    {
        RegisterPluginStep<Task>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<IActivityService>().DemoValidateTask());

        RegisterPluginStep<Task>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<IActivityService>().DemoValidateTask())
            .AddFilteredAttributes(
                t => t.OwnerId,
                t => t.ScheduledStart)
            .AddImage(
                ImageType.PreImage,
                t => t.OwnerId,
                t => t.ScheduledStart);
    }
}