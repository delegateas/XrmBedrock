using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace DataverseRegistration.EconomyArea;

public class InvoiceCollectionStatusChange : Plugin
{
    public InvoiceCollectionStatusChange()
    {
        RegisterPluginStep<ctx_InvoiceCollection>(
            EventOperation.Update,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<InvoiceCollectionService>().HandleStatusChange())
        .AddImage(ImageType.PostImage)
        .AddFilteredAttributes(
            x => x.statuscode);
    }
}