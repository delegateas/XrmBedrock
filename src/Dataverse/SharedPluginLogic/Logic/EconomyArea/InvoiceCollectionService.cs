using DataverseLogic.Azure;
using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class InvoiceCollectionService : IInvoiceCollectionService
{
    private readonly IPluginExecutionContext pluginContext;
    private readonly IAzureService azureService;

    public InvoiceCollectionService(
        IPluginExecutionContext pluginContext,
        IAzureService azureService)
    {
        this.pluginContext = pluginContext;
        this.azureService = azureService;
    }

    public void HandleStatusChange()
    {
        var target = pluginContext.GetPostImageDefaultTarget<mgs_InvoiceCollection>();

        if (target.statuscode != mgs_InvoiceCollection_statuscode.CreateInvoices)
            return;

        if (target.mgs_InvoiceUntil == null)
            throw new InvalidPluginExecutionException("Invoice Until is required");

        azureService.SendCreateInvoicesMessage(new SharedDomain.EconomyArea.CreateInvoicesMessage(target.mgs_InvoiceUntil.Value, target.Id));
    }
}