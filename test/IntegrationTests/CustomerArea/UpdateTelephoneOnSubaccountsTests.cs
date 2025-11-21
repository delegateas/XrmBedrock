using XrmBedrock.SharedContext;

namespace IntegrationTests.CustomerArea;

public class UpdateTelephoneOnSubaccountsTests : TestBase
{
    public UpdateTelephoneOnSubaccountsTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void Should_Update_Telephone1_On_Subaccounts_When_Parent_Telephone_Changes()
    {
        // Arrange
        var parentAccount = Producer.ProduceValidAccount(new Account { Telephone1 = "+45 1234 5678" });
        var subAccount1 = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), Telephone1 = "+45 1234 5678" });
        var subAccount2 = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), Telephone1 = "+45 1234 5678" });
        var subAccount3 = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), Telephone1 = "+45 9876 5432" });

        // Act - Update parent's telephone
        var updatedParent = new Account { Id = parentAccount.Id, Telephone1 = "+45 8765 4321" };
        AdminDao.Update(updatedParent);

        // Assert - Check subaccounts
        var updatedSubAccount1Telephone = AdminDao.Retrieve<Account, string>(subAccount1.Id, a => a.Telephone1);
        updatedSubAccount1Telephone.Should().Be("+45 8765 4321", "Subaccount1's telephone should be updated to match the parent's new telephone.");

        var updatedSubAccount2Telephone = AdminDao.Retrieve<Account, string>(subAccount2.Id, a => a.Telephone1);
        updatedSubAccount2Telephone.Should().Be("+45 8765 4321", "Subaccount2's telephone should be updated to match the parent's new telephone.");

        var updatedSubAccount3Telephone = AdminDao.Retrieve<Account, string>(subAccount3.Id, a => a.Telephone1);
        updatedSubAccount3Telephone.Should().Be("+45 9876 5432", "Subaccount3's telephone should remain unchanged as it did not match the parent's original telephone.");
    }

    [Fact]
    public void Should_Not_Update_Telephone1_On_Subaccounts_When_Parent_Telephone_Unchanged()
    {
        // Arrange
        var parentAccount = Producer.ProduceValidAccount(new Account { Telephone1 = "+45 1234 5678" });
        var subAccount = Producer.ProduceValidAccount(new Account { ParentAccountId = parentAccount.ToEntityReference(), Telephone1 = "+45 1234 5678" });

        // Act - Update parent without changing telephone
        var updatedParent = new Account { Id = parentAccount.Id, Name = "Updated Name" };
        AdminDao.Update(updatedParent);

        // Assert - Check subaccount telephone remains unchanged
        var updatedSubAccountTelephone = AdminDao.Retrieve<Account, string>(subAccount.Id, a => a.Telephone1);
        updatedSubAccountTelephone.Should().Be("+45 1234 5678", "Subaccount's telephone should remain unchanged as parent's telephone was not updated.");
    }
}
