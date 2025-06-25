using Dataverse.PluginLogic.ExampleCustomerArea;
using Dataverse.Plugins;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Delegateas.XrmBedrock.SharedPluginLogic.Plugins.ExampleCustomerArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
///
/// A good practice is to describe what a plugin does in natuaral languate. For this plugin it would be something like:
/// This plugin copies the Telephone1 field to the Account entity from the parent if it is emtpy and the parent has a Telephone1 value.
/// </summary>
public class ExampleCopyParentTelephone : Plugin
{
    public ExampleCopyParentTelephone()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PreOperation,
            provider => provider.GetRequiredService<ExampleCustomerService>().CopyParentTelephone());
    }
}