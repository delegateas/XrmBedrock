using Dataverse.PluginLogic.ExampleCustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.CustomerArea;

/// <summary>
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