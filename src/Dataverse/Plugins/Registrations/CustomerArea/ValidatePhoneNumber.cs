using DataverseLogic.CustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace DataverseRegistration.CustomerArea;

public class ValidatePhoneNumber : Plugin
{
    public ValidatePhoneNumber()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<CustomerService>().ValidatePhoneNumber());

        RegisterPluginStep<Account>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<CustomerService>().ValidatePhoneNumber())
            .AddFilteredAttributes(
                a => a.Telephone1)
            .AddImage(
                ImageType.PreImage,
                a => a.Telephone1);
    }
}
