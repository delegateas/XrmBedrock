using DataverseLogic.EconomyArea;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.EconomyArea;

public class SubscriptionValidation : Plugin
{
    public SubscriptionValidation()
    {
        RegisterPluginStep<mgs_Subscription>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ISubscriptionService>().ValidateSubscription())
        .AddFilteredAttributes(Attributes);

        RegisterPluginStep<mgs_Subscription>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ISubscriptionService>().ValidateSubscription())
        .AddImage(ImageType.PreImage, Attributes)
        .AddFilteredAttributes(Attributes);
    }

    private static Expression<Func<mgs_Subscription, object?>>[] Attributes => new Expression<Func<mgs_Subscription, object?>>[]
    {
        x => x.mgs_StartDate,
        x => x.mgs_EndDate,
    };
}