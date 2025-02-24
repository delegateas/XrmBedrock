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
        // Get post image from the Plugin Context, but default to the target if the post image is not available
        var target = pluginContext.GetPostImageDefaultTarget<mgs_InvoiceCollection>();

        // If the status code is not CreateInvoices, return
        if (target.statuscode != mgs_InvoiceCollection_statuscode.CreateInvoices)
            return;

        // If the Invoice Until is not set, throw an exception
        if (target.mgs_InvoiceUntil == null)
            throw new InvalidPluginExecutionException("Invoice Until is required");

        // Send Create Invoices message to the storage queue in Azure
        azureService.SendCreateInvoicesMessage(new SharedDomain.EconomyArea.CreateInvoicesMessage(target.mgs_InvoiceUntil.Value, target.Id));
    }
}