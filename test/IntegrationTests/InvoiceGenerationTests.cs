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
        var subscription = Producer.ProduceValidSubscription(new mgs_Subscription()
        {
            mgs_StartDate = DateTime.Now.AddMonths(-1),
            mgs_Product = Producer.ProduceValidProduct(new mgs_Product()
            {
                mgs_BillingInterval = mgs_billinginterval.Monthly,
            }).ToEntityReference(),
        });

        // Create an invoice collection using the producer with default values
        var invoiceCollection = Producer.ProduceValidInvoiceCollection(null);

        // Assert that default values are set as expected by the test
        invoiceCollection.mgs_Name.Should().NotBeNull();

        // Trigger plugin that sends message to Azure
        AdminDao.Update(new mgs_InvoiceCollection(invoiceCollection.Id)
        {
            statuscode = mgs_InvoiceCollection_statuscode.CreateInvoices,
        });

        // Send messages to Azure
        await MessageExecutor.SendMessages();

        // Assert that the invoice collection status is updated
        var retrievedInvoiceCollection = AdminDao.Retrieve<mgs_InvoiceCollection>(invoiceCollection.Id, x => x.statuscode);
        retrievedInvoiceCollection.statuscode.Should().Be(mgs_InvoiceCollection_statuscode.InvoicesCreated);

        // Assert that a single transaction was created from the Custom API
        var transactions = AdminDao.RetrieveList(xrm => xrm.mgs_TransactionSet);
        transactions.Count.Should().Be(1);
        transactions[0].mgs_Subscription.Id.Should().Be(subscription.Id);

        // Assert that the invoice was created after the call to the Custom API
        var invoice = AdminDao.Retrieve<mgs_Invoice>(transactions[0].mgs_Invoice.Id, x => x.mgs_InvoiceCollection);
        invoice.mgs_InvoiceCollection.Id.Should().Be(invoiceCollection.Id);
    }
}