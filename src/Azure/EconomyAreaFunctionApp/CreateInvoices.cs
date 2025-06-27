using DataverseService.EconomyArea;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedDomain;
using SharedDomain.EconomyArea;

namespace EconomyAreaFunctionApp;

public class CreateInvoices
{
    private readonly ILogger<CreateInvoices> logger;
    private readonly IDataverseInvoiceService dataverseInvoiceService;

    public CreateInvoices(
        ILogger<CreateInvoices> logger,
        IDataverseInvoiceService dataverseInvoiceService)
    {
        this.logger = logger;
        this.dataverseInvoiceService = dataverseInvoiceService;
    }

    [Function(nameof(CreateInvoices))]
    public Task Run([QueueTrigger(QueueNames.CreateInvoicesQueue)] CreateInvoicesMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        logger.LogInformation($"C# Queue trigger function processed: {message.End}");

        return dataverseInvoiceService.CreateInvoices(message.End, message.InvoiceCollectionId);
    }
}