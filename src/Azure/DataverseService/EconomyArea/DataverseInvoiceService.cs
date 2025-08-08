using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseService.EconomyArea;

public class DataverseInvoiceService : IDataverseInvoiceService
{
    private readonly IDataverseAccessObjectAsync adminDao;
    private readonly IDataverseCustomApiService customApiService;

    public DataverseInvoiceService(
        IDataverseAccessObjectAsync adminDao,
        IDataverseCustomApiService customApiService)
    {
        this.adminDao = adminDao;
        this.customApiService = customApiService;
    }

    public async Task CreateInvoices(DateTime invoiceUntil, Guid invoiceCollectionId)
    {
        await CreateTransactions(invoiceUntil);
        await GroupTransactionsOnInvoice(invoiceCollectionId);
    }

    private async Task GroupTransactionsOnInvoice(Guid invoiceCollectionId)
    {
        var ungroupedTransactions = await adminDao.RetrieveListAsync(xrm =>
            from transaction in xrm.demo_TransactionSet
            join membership in xrm.demo_MembershipSet on transaction.demo_Membership!.Id equals membership.Id
            where transaction.demo_Invoice == null
            where membership.demo_Contact != null
            select new
            {
                TransactionId = transaction.demo_TransactionId,
                ContactId = membership.demo_Contact!.Id,
            });

        var groupedTransactions = ungroupedTransactions.GroupBy(x => x.ContactId);
        foreach (var group in groupedTransactions)
        {
            var invoice = new demo_Invoice()
            {
                demo_InvoiceCollection = new EntityReference(demo_InvoiceCollection.EntityLogicalName, invoiceCollectionId),
                demo_Contact = new EntityReference(Contact.EntityLogicalName, group.Key),
            };

            var invoiceId = await adminDao.CreateAsync(invoice);

            foreach (var transaction in group)
            {
                var transactionEntity = new demo_Transaction(transaction.TransactionId)
                {
                    demo_Invoice = new EntityReference(demo_Invoice.EntityLogicalName, invoiceId),
                };

                await adminDao.UpdateAsync(transactionEntity);
            }
        }

        adminDao.Update(new demo_InvoiceCollection(invoiceCollectionId)
        {
            statuscode = demo_invoicecollection_statuscode.Invoices_Created,
        });
    }

    private async Task CreateTransactions(DateTime invoiceUntil)
    {
        var subscriptions = await adminDao.RetrieveListAsync(xrm =>
            xrm.demo_MembershipSet
            .Where(x => x.demo_InvoicedUntil == null || x.demo_InvoicedUntil < invoiceUntil));

        foreach (var subscription in subscriptions)
        {
            await customApiService.CreateTransactions(new SharedDomain.EconomyArea.CreateTransactionsRequest(subscription.Id, invoiceUntil));
        }
    }
}