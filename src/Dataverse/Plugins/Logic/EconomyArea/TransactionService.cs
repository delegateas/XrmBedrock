using DataverseLogic.EconomyArea.TransactionCalculation;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain.EconomyArea;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class TransactionService
{
    private readonly IPluginExecutionContext context;
    private readonly IAdminDataverseAccessObjectService adminDao;

    public TransactionService(
        IPluginExecutionContext context,
        IAdminDataverseAccessObjectService adminDao)
    {
        this.context = context;
        this.adminDao = adminDao;
    }

    public void CreateTransactionsFromPayload()
    {
        // Get request object from the Plugin Context
        var request = context.GetRequest<CreateTransactionsRequest>();

        // Retrieve subscription related to the request
        var subscription = adminDao.Retrieve<ctx_Subscription>(request.SubscriptionId, x => x.ctx_InvoicedUntil, x => x.ctx_EndDate, x => x.ctx_Product, x => x.ctx_StartDate);

        // If the subscription is already invoiced until the end of the request, return
        if (subscription.ctx_InvoicedUntil >= request.End)
            return;

        // If the subscription product is not set, throw an exception
        if (subscription.ctx_Product == null)
            throw new InvalidPluginExecutionException("Subscription product is required");

        // Retrieve product related to the subscription
        var product = adminDao.Retrieve<ctx_Product>(subscription.ctx_Product.Id, x => x.ctx_BillingInterval, x => x.ctx_Price);

        // If the product billing interval is not set, throw an exception
        if (product.ctx_BillingInterval == null)
            throw new InvalidPluginExecutionException("Product billing interval is required");

        // Create calculation service based on the product billing interval using the calculation service factory
        var calculationService = new TransactionCalculationFactory(product.ctx_BillingInterval.Value).GetCalculationService();

        // Get the start date for the calculation
        var start = subscription.ctx_InvoicedUntil ?? subscription.ctx_StartDate;
        if (start == null)
            throw new InvalidPluginExecutionException("Subscription start is required");

        // Get the end date for the calculation
        var end = subscription.ctx_EndDate ?? request.End;

        // Calculate transactions based on the start and end date
        var transactions = calculationService.CalculateTransactions(start.Value, end);

        // Create transactions based on the calculated transactions
        foreach (var transaction in transactions)
        {
            var entity = new ctx_Transaction
            {
                ctx_Subscription = subscription.ToEntityReference(),
                ctx_Amount = product.ctx_Price,
                ctx_StartDate = transaction.Start,
                ctx_EndDate = transaction.End,
            };

            adminDao.Create(entity);
        }

        // Update the subscription invoiced until date to the end of the request such that the subscription is not invoiced again from Azure
        adminDao.Update(new ctx_Subscription(subscription.Id)
        {
            ctx_InvoicedUntil = end,
        });
    }
}
