using DataverseLogic.EconomyArea;
using Microsoft.Extensions.DependencyInjection;

namespace DataverseRegistration.EconomyArea;

public class CreateTransactions : CustomAPI
{
    public CreateTransactions()
    {
        RegisterCustomAPI("CreateTransactions", provider => provider.GetRequiredService<TransactionService>().CreateTransactionsFromPayload())
            .AddRequestParameter(new CustomAPIConfig.CustomAPIRequestParameter("Payload", RequestParameterType.String));
    }
}