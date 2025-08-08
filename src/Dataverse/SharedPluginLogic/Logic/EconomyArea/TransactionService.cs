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
        var membership = adminDao.Retrieve<demo_Membership>(request.SubscriptionId, x => x.demo_InvoicedUntil, x => x.demo_EndDate, x => x.demo_Product, x => x.demo_StartDate);

        // If the subscription is already invoiced until the end of the request, return
        if (membership.demo_InvoicedUntil >= request.End)
            return;

        // If the subscription product is not set, throw an exception
        if (membership.demo_Product == null)
            throw new InvalidPluginExecutionException("Subscription product is required");

        // Retrieve product related to the subscription
        var product = adminDao.Retrieve<demo_Product>(membership.demo_Product.Id, x => x.demo_BillingInterval, x => x.demo_Price);

        // If the product billing interval is not set, throw an exception
        if (product.demo_BillingInterval == null)
            throw new InvalidPluginExecutionException("Product billing interval is required");

        // Create calculation service based on the product billing interval using the calculation service factory
        var calculationService = new TransactionCalculationFactory(product.demo_BillingInterval.Value.ToDomain()).GetCalculationService();

        // Get the start date for the calculation
        var start = membership.demo_InvoicedUntil ?? membership.demo_StartDate;
        if (start == null)
            throw new InvalidPluginExecutionException("Subscription start is required");

        // Get the end date for the calculation
        var end = membership.demo_EndDate ?? request.End;

        // Calculate transactions based on the start and end date
        var transactions = calculationService.CalculateTransactions(start.Value, end);

        // Create transactions based on the calculated transactions
        foreach (var transaction in transactions)
        {
            var entity = new demo_Transaction
            {
                demo_Membership = membership.ToEntityReference(),
                demo_Amount = product.demo_Price,
                demo_Start = transaction.Start,
                demo_End = transaction.End,
                demo_Type = demo_transaction_demo_type.Product,
            };

            adminDao.Create(entity);
        }

        // Update the subscription invoiced until date to the end of the request such that the subscription is not invoiced again from Azure
        adminDao.Update(new demo_Membership(membership.Id)
        {
            demo_InvoicedUntil = end,
        });
    }
}