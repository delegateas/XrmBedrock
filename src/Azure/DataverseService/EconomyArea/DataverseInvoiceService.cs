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
            from transaction in xrm.mgs_TransactionSet
            join subscription in xrm.mgs_SubscriptionSet on transaction.mgs_Subscription.Id equals subscription.Id
            where transaction.mgs_Invoice == null
            where subscription.mgs_Contact != null
            select new
            {
                TransactionId = transaction.mgs_TransactionId ?? Guid.Empty,
                ContactId = subscription.mgs_Contact.Id,
            });

        var groupedTransactions = ungroupedTransactions.GroupBy(x => x.ContactId);
        foreach (var group in groupedTransactions)
        {
            var invoice = new mgs_Invoice
            {
                mgs_InvoiceCollection = new EntityReference(mgs_InvoiceCollection.EntityLogicalName, invoiceCollectionId),
                mgs_Contact = new EntityReference(Contact.EntityLogicalName, group.Key),
            };

            var invoiceId = await adminDao.CreateAsync(invoice);

            foreach (var transaction in group)
            {
                var transactionEntity = new mgs_Transaction(transaction.TransactionId)
                {
                    mgs_Invoice = new EntityReference(mgs_Invoice.EntityLogicalName, invoiceId),
                };

                await adminDao.UpdateAsync(transactionEntity);
            }
        }

        adminDao.Update(new mgs_InvoiceCollection(invoiceCollectionId)
        {
            statuscode = mgs_InvoiceCollection_statuscode.InvoicesCreated,
        });
    }

    private async Task CreateTransactions(DateTime invoiceUntil)
    {
        var subscriptions = await adminDao.RetrieveListAsync(xrm =>
            xrm.mgs_SubscriptionSet
            .Where(x => x.mgs_InvoicedUntil == null || x.mgs_InvoicedUntil < invoiceUntil));

        foreach (var subscription in subscriptions)
        {
            await customApiService.CreateTransactions(new SharedDomain.EconomyArea.CreateTransactionsRequest(subscription.Id, invoiceUntil));
        }
    }
}