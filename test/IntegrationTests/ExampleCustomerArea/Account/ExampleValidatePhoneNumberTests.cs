using Microsoft.Xrm.Sdk;
using XrmBedrock.SharedContext;

namespace IntegrationTests.ExampleCustomerArea;

public class ExampleValidatePhoneNumberTests : TestBase
{
    public ExampleValidatePhoneNumberTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("+123 456 7890")]
    public void ValidatePhoneNumber_DoesNotThrowException(string? phoneNumber)
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account { Telephone1 = phoneNumber });

        // Act
        var accountId = AdminDao.Create(account);

        // Assert
        var fetchedPhoneNumber = AdminDao.Retrieve<Account, string?>(accountId, a => a.Telephone1);
        if (string.IsNullOrEmpty(phoneNumber))
            fetchedPhoneNumber.Should().BeNull();
        else
            fetchedPhoneNumber.Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+123-456-7890")]
    [InlineData("+123abc")]
    public void ValidatePhoneNumber_ThrowsException(string? phoneNumber)
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account { Telephone1 = phoneNumber });

        // Act
        var act = () => AdminDao.Create(account);

        // Assert
        act.Should()
        .Throw<InvalidPluginExecutionException>()
        .WithMessage("The phone number must start with '+' and contain only digits and spaces.");
    }
}