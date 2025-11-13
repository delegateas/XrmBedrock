using DataverseLogic.Azure;
using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class InvoiceCollectionService
{
    private readonly IPluginExecutionContext pluginContext;
    private readonly AzureService azureService;

    public InvoiceCollectionService(
        IPluginExecutionContext pluginContext,
        AzureService azureService)
    {
        this.pluginContext = pluginContext;
        this.azureService = azureService;
    }

    public void HandleStatusChange()
    {
        // Get post image from the Plugin Context, but default to the target if the post image is not available
        var target = pluginContext.GetPostImageDefaultTarget<ctx_InvoiceCollection>();

        // If the status code is not CreateInvoices, return
        if (target.statuscode != ctx_InvoiceCollection_statuscode.CreateInvoices)
            return;

        // If the Invoice Until is not set, throw an exception
        if (target.ctx_InvoiceUntil == null)
            throw new InvalidPluginExecutionException("Invoice Until is required");

        // Send Create Invoices message to the storage queue in Azure
        azureService.SendCreateInvoicesMessage(new SharedDomain.EconomyArea.CreateInvoicesMessage(target.ctx_InvoiceUntil.Value, target.Id));
    }
}