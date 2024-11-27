using DataverseService.Dto.Account;

namespace MedlemssystemApi.BusinessLogic;

public interface IAccountBusinessLogic
{
    Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccountRequest);
}