using DataverseLogic.EconomyArea;
using Microsoft.Extensions.DependencyInjection;

namespace Dataverse.Plugins.APIs;

public class CreateTransactions : CustomAPI
{
    public CreateTransactions()
    {
        RegisterCustomAPI("CreateTransactions", provider => provider.GetRequiredService<ITransactionService>().CreateTransactionsFromPayload())
            .AddRequestParameter(new CustomAPIConfig.CustomAPIRequestParameter("Payload", RequestParameterType.String));
    }
}