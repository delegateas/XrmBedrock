using Dataverse.PluginLogic.ExampleActivityArea;
using Microsoft.Extensions.DependencyInjection;
using Task = XrmBedrock.SharedContext.Task;

namespace Dataverse.Plugins.ActivityArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// Them main illustration here is actually in the tests, which show how to test situation where the user context matters.
/// By additionally adding specific roles to the user in the tests you can test the behavior of the plugin that depend on user roles.
///
/// A good practice is to describe what a plugin does in natural language. For this plugin it would be something like:
/// This plugin is responsible for validating that a task assigned to someone else is scheduled within business hours.
/// </summary>
public class ExampleValidateTask : Plugin
{
    public ExampleValidateTask()
    {
        RegisterPluginStep<Task>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ExampleActivityService>().ExampleValidateTaskIsWithinBusinessHours());

        RegisterPluginStep<Task>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ExampleActivityService>().ExampleValidateTaskIsWithinBusinessHours())
            .AddFilteredAttributes(
                t => t.OwnerId,
                t => t.ScheduledStart)
            .AddImage(
                ImageType.PreImage,
                t => t.OwnerId,
                t => t.ScheduledStart);
    }
}