using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseService.EconomyArea;

public class DataverseInvoiceService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseInvoiceService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }

    public async Task CreateInvoices(DateTime invoiceUntil, Guid invoiceCollectionId)
    {
        await CreateTransactions(invoiceUntil);
        await GroupTransactionsOnInvoice(invoiceCollectionId);
    }

    private async Task GroupTransactionsOnInvoice(Guid invoiceCollectionId)
    {
        var ungroupedTransactions = await adminDao.RetrieveListAsync(xrm =>
            from transaction in xrm.ctx_TransactionSet
            join subscription in xrm.ctx_SubscriptionSet on transaction.ctx_Subscription.Id equals subscription.Id
            where transaction.ctx_Invoice == null
            where subscription.ctx_Customer != null
            select new
            {
                TransactionId = transaction.ctx_TransactionId ?? Guid.Empty,
                ContactId = subscription.ctx_Customer.Id,
            });

        var groupedTransactions = ungroupedTransactions.GroupBy(x => x.ContactId);
        foreach (var group in groupedTransactions)
        {
            var invoice = new ctx_Invoice
            {
                ctx_InvoiceCollection = new EntityReference(ctx_InvoiceCollection.EntityLogicalName, invoiceCollectionId),
                ctx_Customer = new EntityReference(Contact.EntityLogicalName, group.Key),
            };

            var invoiceId = await adminDao.CreateAsync(invoice);

            foreach (var transaction in group)
            {
                var transactionEntity = new ctx_Transaction(transaction.TransactionId)
                {
                    ctx_Invoice = new EntityReference(ctx_Invoice.EntityLogicalName, invoiceId),
                };

                await adminDao.UpdateAsync(transactionEntity);
            }
        }

        adminDao.Update(new ctx_InvoiceCollection(invoiceCollectionId)
        {
            statuscode = ctx_InvoiceCollection_statuscode.InvoicesCreated,
        });
    }

    private async Task CreateTransactions(DateTime invoiceUntil)
    {
        var subscriptions = await adminDao.RetrieveListAsync(xrm =>
            xrm.ctx_SubscriptionSet
            .Where(x => x.ctx_InvoicedUntil == null || x.ctx_InvoicedUntil < invoiceUntil));

        foreach (var subscription in subscriptions)
        {
            await adminDao.ExecuteAsync(new OrganizationRequest("ctx_CreateTransactions")
            {
                Parameters = { { "Payload", JsonConvert.SerializeObject(new SharedDomain.EconomyArea.CreateTransactionsRequest(subscription.Id, invoiceUntil)) } },
            });
        }
    }
}