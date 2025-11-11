using DataverseLogic.CustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace DataverseRegistration.CustomerArea;

public class CopyParentTelephone : Plugin
{
    public CopyParentTelephone()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PreOperation,
            provider => provider.GetRequiredService<CustomerService>().CopyParentTelephone());
    }
}