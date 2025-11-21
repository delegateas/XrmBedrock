using XrmBedrock.SharedContext;

namespace IntegrationTests.CustomerArea;

public class CopyParentTelephoneTests : TestBase
{
    public CopyParentTelephoneTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void Should_Copy_Telephone1_From_Parent_When_Not_Set()
    {
        // Arrange
        var parentAccount = Producer.ProduceValidAccount(new Account { Telephone1 = "+1234567890", });

        // Act
        // Telephone1 is not set on the new account
        var newAccount = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), });

        // Assert
        var createdAccountTelephone = AdminDao.Retrieve<Account, string>(newAccount.Id, a => a.Telephone1);
        createdAccountTelephone.Should().Be("+1234567890", "Telephone1 should be copied from the parent account when not set on the new account.");
    }

    [Fact]
    public void Should_Not_Copy_Telephone1_From_Parent_When_Already_Set()
    {
        // Arrange
        var parentAccount = Producer.ProduceValidAccount(new Account { Telephone1 = "+1234567890", });

        // Act
        // Telephone1 is set on the new account
        var newAccount = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), Telephone1 = "+0987654321", });

        // Assert
        var createdAccountTelephone = AdminDao.Retrieve<Account, string>(newAccount.Id, a => a.Telephone1);
        createdAccountTelephone.Should().Be("+0987654321", "Telephone1 should not be copied from the parent account when already set on the new account.");
    }

    [Fact]
    public void Should_Not_Attempt_Copy_When_No_Parent_Account()
    {
        // Arrange
        // Act
        var newAccount = Producer.ProduceValidAccount(null);

        // Assert
        var createdAccountTelephone = AdminDao.Retrieve<Account, string>(newAccount.Id, a => a.Telephone1);
        createdAccountTelephone.Should().BeNullOrEmpty("Telephone1 should remain unset when there is no parent account.");
    }
}
