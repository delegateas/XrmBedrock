using DataverseLogic.EconomyArea.TransactionCalculation;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using SharedContext.Dao;
using SharedDomain.EconomyArea;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class TransactionService : ITransactionService
{
    private readonly IPluginExecutionContext context;
    private readonly IAdminDataverseAccessObjectService adminDao;
    private readonly ILogger logger;

    public TransactionService(
        IPluginExecutionContext context,
        IAdminDataverseAccessObjectService adminDao,
        ILogger logger)
    {
        this.context = context;
        this.adminDao = adminDao;
        this.logger = logger;
    }

    public void CreateTransactionFromPayload()
    {
        var requestString = context.InputParameters["Payload"] as string ?? throw new InvalidPluginExecutionException($"Payload parameter required");

        logger.LogInformation($"Request: {requestString}");

        var request = JsonConvert.DeserializeObject<CreateTransactionsRequest>(requestString) ?? throw new InvalidPluginExecutionException($"Invalid request format {requestString}");

        var subscription = adminDao.Retrieve<mgs_Subscription>(request.SubscriptionId, x => x.mgs_InvoicedUntil, x => x.mgs_EndDate);

        if (subscription.mgs_InvoicedUntil >= request.End)
            return;

        if (subscription.mgs_Product == null)
            throw new InvalidPluginExecutionException("Subscription product is required");

        var product = adminDao.Retrieve<mgs_Product>(subscription.mgs_Product.Id, x => x.mgs_BillingInterval, x => x.mgs_Price);
        if (product.mgs_BillingInterval == null)
            throw new InvalidPluginExecutionException("Product billing interval is required");

        var calculationService = new TransactionCalculationFactory(product.mgs_BillingInterval.Value.ToDomain()).Create();

        var start = subscription.mgs_InvoicedUntil ?? subscription.mgs_StartDate;
        if (start == null)
            throw new InvalidPluginExecutionException("Subscription start is required");

        var end = subscription.mgs_EndDate ?? request.End;

        var transactions = calculationService.CalculateTransactions(start.Value, end);

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
        
        adminDao.Update(new mgs_Subscription(subscription.Id)
        {
            mgs_InvoicedUntil = end,
        });
    }
}
