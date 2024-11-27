using DataverseService.Dto.Account;

namespace DataverseService.CustomerArea;

public interface IDataverseAccountService
{
    Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccount);
}