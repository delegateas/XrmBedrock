using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea;

public class SubscriptionService : ISubscriptionService
{
    private readonly IPluginExecutionContext context;
    private readonly ILogger logger;

    public SubscriptionService(
        IPluginExecutionContext context,
        ILogger logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public void ValidateSubscription()
    {
        logger.LogInformation("Validating subscription");

        var subscription = context.GetTargetMergedWithPreImage<mgs_Subscription>();
        if (subscription.mgs_EndDate == null)
            return;

        if (subscription.mgs_StartDate == null)
            throw new InvalidPluginExecutionException("Start date is required");

        if (subscription.mgs_StartDate > subscription.mgs_EndDate)
            throw new InvalidPluginExecutionException("Start date cannot be after end date");
    }
}