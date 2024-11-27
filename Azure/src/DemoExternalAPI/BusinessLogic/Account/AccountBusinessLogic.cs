using DataverseService.CustomerArea;
using DataverseService.Dto.Account;
using DataverseService.Foundation.Logging;

namespace DemoExternalApi.BusinessLogic;

public class AccountBusinessLogic : IAccountBusinessLogic
{
    private readonly IDataverseAccountService dataverseAccountService;
    private readonly ILogger<AccountBusinessLogic> logger;

    public AccountBusinessLogic(IDataverseAccountService dataverseAccountService, ILogger<AccountBusinessLogic> logger)
    {
        this.dataverseAccountService = dataverseAccountService;
        this.logger = logger;
    }

    public Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccountRequest)
    {
        logger.LogMessageInformation("Adding account to Dataverse");
        return dataverseAccountService.CreateAccount(createAccountRequest);
    }
}