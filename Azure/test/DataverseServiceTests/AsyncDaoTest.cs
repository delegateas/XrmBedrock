using LF.Medlemssystem.DataverseTests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseServiceTests;

/// <summary>
/// This is not a covering test of all Async menthods but just a few sample tests
/// </summary>
public class AsyncDaoTest : TestBase
{
    public AsyncDaoTest(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task TestCreateAsync()
    {
        // Arrange
        var someContactObject = Producer.ConstructValidContact(null);

        // Act
        var someAccount = await AdminDao.CreateAsync(someContactObject);

        // Assert
        someAccount.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task TestRetrieveAsync()
    {
        // Arrange
        var someName = "MyTest";
        var someAccount = Producer.ProduceValidAccount(new Account() { Name = someName, });
        var someAccountId = someAccount.Id;

        // Act
        var someAccountName = await AdminDao.RetrieveAsync<Account, string>(someAccountId, a => a.Name);

        // Assert
        someAccountName.Should().Be(someName);
    }

    [Fact]
    public async Task TestRetrieveSingleAsync()
    {
        // Arrange
        var someName = "MySigleTest";
        var someAccount = Producer.ProduceValidAccount(new Account() { Name = someName, });

        // Act
        var someAccountId = await AdminDao.RetrieveSingleAsync(xrm => xrm.AccountSet.Where(a => a.Name == someName).Select(a => a.Id));

        // Assert
        someAccountId.Should().Be(someAccount.Id);
    }

    [Fact]
    public async Task TestExecuteUnawaitedAsync()
    {
        // Arrange
        var firstContact = Producer.ConstructValidContact(null);
        var secondContact = Producer.ConstructValidContact(null);
        var thirdContact = Producer.ConstructValidContact(null);
        var executeMultipleRequest = new ExecuteMultipleRequest()
        {
            Settings = new ExecuteMultipleSettings()
            {
                ContinueOnError = false,
                ReturnResponses = true,
            },
            Requests = new OrganizationRequestCollection()
            {
                new CreateRequest() { Target = firstContact, },
                new CreateRequest() { Target = secondContact, },
                new CreateRequest() { Target = thirdContact, },
            },
        };

        // Act
        var response = await AdminDao.ExecuteUnawaitedAsync(executeMultipleRequest) as ExecuteMultipleResponse;

        // Assert
        response.Should().NotBeNull();
        response?.Responses.Count.Should().Be(3);
        response?.Responses[0].Response.Should().BeOfType<CreateResponse>();
        response?.Responses[1].Response.Should().BeOfType<CreateResponse>();
        response?.Responses[2].Response.Should().BeOfType<CreateResponse>();
    }
}