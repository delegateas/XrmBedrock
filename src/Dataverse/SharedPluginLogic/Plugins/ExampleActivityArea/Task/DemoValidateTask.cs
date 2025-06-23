using Task = XrmBedrock.SharedContext.Task;

namespace Dataverse.Plugins.ActivityArea;

/// <summary>
/// !This plugin serves purely as an example for developers and agents and should be removed from the solution, when you have created a few real plugins in project!
/// 
/// This plugin is responsible for validating that a task assigned to someone else is scheduled within business hours.
/// </summary>
public class ExampleValidateTask : Plugin
{
    public ExampleValidateTask()
        : base(typeof(ExampleValidateTask))
    {
        RegisterPluginStep<Task>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<IExampleActivityService>().ExampleValidateTaskIsWithinBusinessHours());

        RegisterPluginStep<Task>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<IExampleActivityService>().ExampleValidateTaskIsWithinBusinessHours())
            .AddFilteredAttributes(
                t => t.OwnerId,
                t => t.ScheduledStart)
            .AddImage(
                ImageType.PreImage,
                t => t.OwnerId,
                t => t.ScheduledStart);
    }
}