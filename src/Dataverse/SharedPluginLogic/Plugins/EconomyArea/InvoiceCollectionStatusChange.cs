using DataverseLogic.EconomyArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.EconomyArea;

public class InvoiceCollectionStatusChange : Plugin
{
    public InvoiceCollectionStatusChange()
    {
        RegisterPluginStep<mgs_InvoiceCollection>(
            EventOperation.Update,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<IInvoiceCollectionService>().HandleStatusChange())
        .AddImage(ImageType.PostImage)
        .AddFilteredAttributes(
            x => x.statuscode);
    }
}