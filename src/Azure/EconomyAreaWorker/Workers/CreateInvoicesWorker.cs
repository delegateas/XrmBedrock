using Azure.Storage.Queues;
using DataverseService.EconomyArea;
using DataverseService.Workers;
using SharedDomain;
using SharedDomain.EconomyArea;

namespace EconomyAreaWorker.Workers;

public sealed class CreateInvoicesWorker(
    IServiceScopeFactory scopeFactory,
    [FromKeyedServices(QueueNames.CreateInvoicesQueue)] QueueClient queueClient,
    ILogger<CreateInvoicesWorker> logger)
    : QueueWorker<CreateInvoicesMessage>(scopeFactory, queueClient, logger)
{
    protected override async Task ProcessMessageAsync(
        CreateInvoicesMessage message,
        IServiceProvider scopedServices,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var invoiceService = scopedServices.GetRequiredService<DataverseInvoiceService>();

        await invoiceService.CreateInvoices(message.End, message.InvoiceCollectionId);

        logger.LogInformation("Processed invoice creation for collection {InvoiceCollectionId}", message.InvoiceCollectionId);
    }
}
