using DataverseService.Dto.Account;

namespace DemoExternalApi.BusinessLogic;

public interface IAccountBusinessLogic
{
    Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccountRequest);
}