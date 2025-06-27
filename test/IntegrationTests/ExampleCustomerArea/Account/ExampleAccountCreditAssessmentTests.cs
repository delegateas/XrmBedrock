using XrmBedrock.SharedContext;

namespace IntegrationTests.ExampleCustomerArea;

public class ExampleAccountCreditAssessmentTests : TestBase
{
    public ExampleAccountCreditAssessmentTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(Account_CustomerTypeCode.Supplier)]
    public void CreateAccount_WithSupplierType_CreatesCreditAssessmentTask(Account_CustomerTypeCode customerType)
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account { CustomerTypeCode = customerType });

        // Act
        var accountId = AdminDao.Create(account);

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == accountId));
        tasks.Should().HaveCount(1, "A credit assessment task should be created for a Supplier account.");
    }

    [Theory]
    [InlineData(Account_CustomerTypeCode.Customer)]
    [InlineData(Account_CustomerTypeCode.Partner)]
    public void CreateAccount_WithNonSupplierType_DoesNotCreateCreditAssessmentTask(Account_CustomerTypeCode customerType)
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account { CustomerTypeCode = customerType });

        // Act
        var accountId = AdminDao.Create(account);

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == accountId));
        tasks.Should().BeEmpty("No credit assessment task should be created for a non-Supplier account.");
    }

    [Fact]
    public void CreateAccount_WithNullCustomerType_DoesNotCreateCreditAssessmentTask()
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account { CustomerTypeCode = null });

        // Act
        var accountId = AdminDao.Create(account);

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == accountId));
        tasks.Should().BeEmpty("No credit assessment task should be created for an account with null Customer Type.");
    }
}