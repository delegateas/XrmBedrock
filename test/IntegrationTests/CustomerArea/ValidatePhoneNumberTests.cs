using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;

namespace IntegrationTests.CustomerArea;

public class ValidatePhoneNumberTests : TestBase
{
    public ValidatePhoneNumberTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("+123 456 7890")]
    public void ValidatePhoneNumber_DoesNotThrowException(string? phoneNumber)
    {
        // Arrange & Act
        var account = Producer.ProduceValidAccount(new Account { Telephone1 = phoneNumber });

        // Assert
        var fetchedPhoneNumber = AdminDao.Retrieve<Account, string>(account.Id, a => a.Telephone1);
        if (string.IsNullOrEmpty(phoneNumber))
        {
            fetchedPhoneNumber.Should().BeNull();
        }
        else
        {
            fetchedPhoneNumber.Should().Be(phoneNumber);
        }
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+123-456-7890")]
    [InlineData("+123abc")]
    public void ValidatePhoneNumber_ThrowsException(string? phoneNumber)
    {
        // Arrange & Act
        var act = () => Producer.ProduceValidAccount(new Account { Telephone1 = phoneNumber });

        // Assert
        act.Should()
        .Throw<InvalidPluginExecutionException>()
        .WithMessage("The phone number must start with '+' and contain only digits and spaces.");
    }
}
