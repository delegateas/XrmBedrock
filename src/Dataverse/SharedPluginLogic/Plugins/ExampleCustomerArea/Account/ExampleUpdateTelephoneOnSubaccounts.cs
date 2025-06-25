using Dataverse.PluginLogic.ExampleCustomerArea;
using Dataverse.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using XrmBedrock.SharedContext;

namespace Delegateas.XrmBedrock.SharedPluginLogic.Plugins.ExampleCustomerArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// Note that it is kind of a doubious example, as it is iterating through a list of chilren doing updates might take a while.
/// In a real scenario of similar nature consider making the plugin run asynchronously by setting ".SetExecutionMode(ExecutionMode.Asynchronous)" or even better throw it on an azure queue and let an azure function do the job.
/// The example does demonstrates the use of Images and Filtered Attributes, which are both important concepts in plugin development.
///
/// A good practice is to describe what a plugin does in natuaral languate. For this plugin it would be something like:This plugin updates the Telephone1 field on subaccounts when the Telephone1 field on the parent Account is changed.
/// It checks if a subaccount's Telephone1 matches the parent's old Telephone1 value, and if so, updates it to the new value.
/// </summary>
public class ExampleUpdateTelephoneOnSubaccounts : Plugin
{
    public ExampleUpdateTelephoneOnSubaccounts()
    {
        RegisterPluginStep<Account>(
            EventOperation.Update,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<ExampleCustomerService>().UpdateTelephoneOnSubaccounts())
            .SetExecutionMode(ExecutionMode.Synchronous) // default, so no need to specify except to illustrate how to change this
            .AddImage(ImageType.PreImage, filterAndImageAttributes)
            .AddImage(ImageType.PostImage, filterAndImageAttributes)
            .AddFilteredAttributes(filterAndImageAttributes);
    }

    private readonly Expression<Func<Account, object?>>[] filterAndImageAttributes =
    {
        x => x.Telephone1,
    };
}