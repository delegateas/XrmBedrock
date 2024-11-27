using Azure.DataverseService.Tests;
using DataverseService.Dto.Account;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseAccountServiceTests;

public class DataverseAccountServiceTest : TestBase
{
    public DataverseAccountServiceTest(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DemoTestCreateAccount()
    {
        // Arrange
        var createAccountRequest = new CreateAccountRequest("New Account", "Example Street");

        // Act
        var response = await DataverseAccountService.CreateAccount(createAccountRequest);

        // Assert
        response.Should().NotBeNull();
        var createdAccount = AdminDao.Retrieve<Account>(response.AccountId, a => a.Name, a => a.Address1_Line1);
        createdAccount.Should().NotBeNull();
        createdAccount.Name.Should().Be(createAccountRequest.Name);
        createdAccount.Address1_Line1.Should().Be(createAccountRequest.StreetName);
    }
}