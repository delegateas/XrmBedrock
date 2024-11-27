using DataverseService.Dto.Account;
using DataverseService.Foundation.Dao;
using XrmBedrock.SharedContext;

namespace DataverseService.CustomerArea;

public class DataverseAccountService : IDataverseAccountService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseAccountService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }

    public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccount)
    {
        ArgumentNullException.ThrowIfNull(createAccount);

        var accountId = await adminDao.CreateAsync(new Account
        {
            Name = createAccount.Name,
            Address1_Line1 = createAccount.StreetName,
        });

        return new CreateAccountResponse(accountId);
    }
}