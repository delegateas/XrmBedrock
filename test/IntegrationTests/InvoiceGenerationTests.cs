using Azure.DataverseService.Tests;
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
    public async Task TestStatusRoundtrip()
    {
        var subscription = Producer.ProduceValidSubscription(new mgs_Subscription()
        {
            mgs_StartDate = DateTime.Now.AddMonths(-1),
            mgs_Product = Producer.ProduceValidProduct(new mgs_Product()
            {
                mgs_BillingInterval = mgs_billinginterval.Monthly,
            }).ToEntityReference(),
        });

        var invoiceCollection = Producer.ProduceValidInvoiceCollection(null);
        invoiceCollection.mgs_Name.Should().NotBeNull();

        AdminDao.Update(new mgs_InvoiceCollection(invoiceCollection.Id)
        {
            statuscode = mgs_InvoiceCollection_statuscode.CreateInvoices,
        });

        await MessageExecutor.SendMessages();

        var retrievedInvoiceCollection = AdminDao.Retrieve<mgs_InvoiceCollection>(invoiceCollection.Id, x => x.statuscode);
        retrievedInvoiceCollection.statuscode.Should().Be(mgs_InvoiceCollection_statuscode.InvoicesCreated);

        var transactions = AdminDao.RetrieveList(xrm => xrm.mgs_TransactionSet);
        transactions.Count.Should().Be(1);
        transactions[0].mgs_Subscription.Id.Should().Be(subscription.Id);

        var invoice = AdminDao.Retrieve<mgs_Invoice>(transactions[0].mgs_Invoice.Id, x => x.mgs_InvoiceCollection);
        invoice.mgs_InvoiceCollection.Id.Should().Be(invoiceCollection.Id);
    }
}