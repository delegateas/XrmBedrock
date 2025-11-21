using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace IntegrationTests.EconomyArea;

public class InvoiceGenerationTests : TestBase
{
    public InvoiceGenerationTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task TestInvoiceGeneration()
    {
        // Create a subscription using the producer with specific values
        var subscription = Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_StartDate = DateTime.Now.AddMonths(-1),
            ctx_Product = Producer.ProduceValidProduct(new ctx_Product
            {
                ctx_BillingInterval = ctx_billinginterval.Monthly,
            }).ToEntityReference(),
        });

        // Create an invoice collection using the producer with default values
        var invoiceCollection = Producer.ProduceValidInvoiceCollection(null);

        // Assert that default values are set as expected by the test
        invoiceCollection.ctx_Name.Should().NotBeNull();

        // Trigger plugin that sends message to Azure
        AdminDao.Update(new ctx_InvoiceCollection(invoiceCollection.Id)
        {
            statuscode = ctx_InvoiceCollection_statuscode.CreateInvoices,
        });

        // Send messages to Azure
        await MessageExecutor.SendMessages();

        // Assert that the invoice collection status is updated
        var retrievedInvoiceCollection = AdminDao.Retrieve<ctx_InvoiceCollection>(invoiceCollection.Id, x => x.statuscode);
        retrievedInvoiceCollection.statuscode.Should().Be(ctx_InvoiceCollection_statuscode.InvoicesCreated);

        // Assert that a single transaction was created from the Custom API
        var transactions = AdminDao.RetrieveList(xrm => xrm.ctx_TransactionSet);
        transactions.Count.Should().Be(1);
        transactions[0].ctx_Subscription.Id.Should().Be(subscription.Id);

        // Assert that the invoice was created after the call to the Custom API
        var invoice = AdminDao.Retrieve<ctx_Invoice>(transactions[0].ctx_Invoice.Id, x => x.ctx_InvoiceCollection);
        invoice.ctx_InvoiceCollection.Id.Should().Be(invoiceCollection.Id);
    }
}
