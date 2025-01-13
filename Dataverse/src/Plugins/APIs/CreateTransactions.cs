using DataverseLogic.EconomyArea;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataverse.Plugins.APIs;

public class CreateTransactions : CustomAPI
{
    public CreateTransactions()
    {
        RegisterCustomAPI("CreateTransactions", provider => provider.GetRequiredService<ITransactionService>().CreateTransactionFromPayload())
            .AddRequestParameter(new CustomAPIConfig.CustomAPIRequestParameter("Payload", RequestParameterType.String));
    }
}