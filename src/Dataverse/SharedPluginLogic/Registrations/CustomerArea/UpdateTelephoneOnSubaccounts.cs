using DataverseLogic.CustomerArea;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using XrmBedrock.SharedContext;

namespace DataverseRegistration.CustomerArea;

public class UpdateTelephoneOnSubaccounts : Plugin
{
    public UpdateTelephoneOnSubaccounts()
    {
        RegisterPluginStep<Account>(
            EventOperation.Update,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<CustomerService>().UpdateTelephoneOnSubaccounts())
            .SetExecutionMode(ExecutionMode.Synchronous) // default, so no need to specify except to illustrate how to change this
            .AddImage(ImageType.PreImage, filterAndImageAttributes)
            .AddImage(ImageType.PostImage, filterAndImageAttributes)
            .AddFilteredAttributes(filterAndImageAttributes);
    }

    private readonly Expression<Func<Account, object?>>[] filterAndImageAttributes =
    {
        x => x.Telephone1,
    };
}