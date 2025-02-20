using DataverseLogic.EconomyArea.TransactionCalculation;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain.EconomyArea;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class TransactionService : ITransactionService
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
        var subscription = adminDao.Retrieve<mgs_Subscription>(request.SubscriptionId, x => x.mgs_InvoicedUntil, x => x.mgs_EndDate, x => x.mgs_Product, x => x.mgs_StartDate);

        // If the subscription is already invoiced until the end of the request, return
        if (subscription.mgs_InvoicedUntil >= request.End)
            return;

        // If the subscription product is not set, throw an exception
        if (subscription.mgs_Product == null)
            throw new InvalidPluginExecutionException("Subscription product is required");

        // Retrieve product related to the subscription
        var product = adminDao.Retrieve<mgs_Product>(subscription.mgs_Product.Id, x => x.mgs_BillingInterval, x => x.mgs_Price);

        // If the product billing interval is not set, throw an exception
        if (product.mgs_BillingInterval == null)
            throw new InvalidPluginExecutionException("Product billing interval is required");

        // Create calculation service based on the product billing interval using the calculation service factory
        var calculationService = new TransactionCalculationFactory(product.mgs_BillingInterval.Value.ToDomain()).GetCalculationService();

        // Get the start date for the calculation
        var start = subscription.mgs_InvoicedUntil ?? subscription.mgs_StartDate;
        if (start == null)
            throw new InvalidPluginExecutionException("Subscription start is required");

        // Get the end date for the calculation
        var end = subscription.mgs_EndDate ?? request.End;

        // Calculate transactions based on the start and end date
        var transactions = calculationService.CalculateTransactions(start.Value, end);

        // Create transactions based on the calculated transactions
        foreach (var transaction in transactions)
        {
            var entity = new mgs_Transaction
            {
                mgs_Subscription = subscription.ToEntityReference(),
                mgs_Amount = product.mgs_Price,
                mgs_Start = transaction.Start,
                mgs_End = transaction.End,
                mgs_Type = mgs_transactiontype.Product,
            };

            adminDao.Create(entity);
        }

        // Update the subscription invoiced until date to the end of the request such that the subscription is not invoiced again from Azure
        adminDao.Update(new mgs_Subscription(subscription.Id)
        {
            mgs_InvoicedUntil = end,
        });
    }
}