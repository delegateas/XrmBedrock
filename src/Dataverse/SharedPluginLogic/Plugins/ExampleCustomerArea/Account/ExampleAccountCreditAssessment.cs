using Dataverse.PluginLogic.ExampleCustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.CustomerArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// This plugin gives a simple demonstration of adding a reated object (here a Task) on creation of an object (here an Account).
/// 
/// A good practice is to describe what a plugin does in natuaral languate. For this plugin it would be something like:
/// This plugin creates a Task for Credit Assessment when an Account is created with Customer Type set to "Supplier".
/// </summary>
public class ExampleAccountCreditAssessment : Plugin
{
    public ExampleAccountCreditAssessment()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<ExampleCustomerService>().CreateCreditAssessmentTask());
    }
}