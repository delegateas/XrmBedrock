using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace IntegrationTests;

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
        var subscription = Producer.ProduceValidSubscription(new demo_Membership()
        {
            demo_StartDate = DateTime.Now.AddMonths(-1),
            demo_Product = Producer.ProduceValidProduct(new demo_Product()
            {
                demo_BillingInterval = demo_billinginterval.Monthly,
            }).ToEntityReference(),
        });

        // Create an invoice collection using the producer with default values
        var invoiceCollection = Producer.ProduceValidInvoiceCollection(null);

        // Assert that default values are set as expected by the test
        invoiceCollection.demo_Name.Should().NotBeNull();

        // Trigger plugin that sends message to Azure
        AdminDao.Update(new demo_InvoiceCollection(invoiceCollection.Id)
        {
            statuscode = demo_invoicecollection_statuscode.Create_Invoices,
        });

        // Send messages to Azure
        await MessageExecutor.SendMessages();

        // Assert that the invoice collection status is updated
        var retrievedInvoiceCollection = AdminDao.Retrieve<demo_InvoiceCollection>(invoiceCollection.Id, x => x.statuscode);
        retrievedInvoiceCollection.statuscode.Should().Be(demo_invoicecollection_statuscode.Invoices_Created);

        // Assert that a single transaction was created from the Custom API
        var transactions = AdminDao.RetrieveList(xrm => xrm.demo_TransactionSet);
        transactions.Count.Should().Be(1);
        transactions[0].demo_Membership?.Id.Should().Be(subscription.Id);

        // Assert that the invoice was created after the call to the Custom API
        transactions[0].demo_Invoice.Should().NotBeNull();
        var invoice = AdminDao.Retrieve<demo_Invoice>(transactions[0].demo_Invoice!.Id, x => x.demo_InvoiceCollection);
        invoice.demo_InvoiceCollection?.Id.Should().Be(invoiceCollection.Id);
    }
}