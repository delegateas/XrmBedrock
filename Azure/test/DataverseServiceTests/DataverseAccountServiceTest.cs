using DataverseService.Dto.Account;
using LF.Medlemssystem.DataverseTests;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseAccountServiceTests
{
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
            var CreateAccountRequest = new CreateAccountRequest("New Account", "Example Street");

            // Act
            var response = await DataverseAccountService.CreateAccount(CreateAccountRequest);

            // Assert
            response.Should().NotBeNull();
            var createdAccount = AdminDao.Retrieve<Account>(response.AccountId);
            createdAccount.Should().NotBeNull();
            createdAccount.Name.Should().Be(CreateAccountRequest.Name);
            createdAccount.Address1_Line1.Should().Be(CreateAccountRequest.StreetName);
        }
    }
}