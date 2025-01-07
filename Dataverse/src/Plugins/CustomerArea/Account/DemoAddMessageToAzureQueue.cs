using DataverseLogic.CustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.CustomerArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// It is demonstratinbg the mechanism of adding a message to an Azure Queue. This is handy for complex operations, integrations , etc.
/// An Azure function will take the message from the queue and process it. The demo here will just add a note to the account.
/// </summary>
internal class DemoAddMessageToAzureQueue : Plugin
{
    public DemoAddMessageToAzureQueue()
        : base(typeof(DemoAddMessageToAzureQueue))
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<ICustomerService>().DemoAddMessageToAzureQueueOnCreate());

        RegisterPluginStep<Account>(
            EventOperation.Update,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<ICustomerService>().DemoAddMessageToAzureQueueOnUpdate())
            .AddFilteredAttributes(
                a => a.PrimaryContactId)
            .AddImage(
                ImageType.PreImage,
                a => a.PrimaryContactId);
    }
}