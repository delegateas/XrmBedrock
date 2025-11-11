using DataverseLogic.CustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace DataverseRegistration.CustomerArea;

public class AccountCreditAssessment : Plugin
{
    public AccountCreditAssessment()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<CustomerService>().CreateCreditAssessmentTask());
    }
}