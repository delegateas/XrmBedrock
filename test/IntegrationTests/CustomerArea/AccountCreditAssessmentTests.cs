using XrmBedrock.SharedContext;

namespace IntegrationTests.CustomerArea;

public class AccountCreditAssessmentTests : TestBase
{
    public AccountCreditAssessmentTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(Account_CustomerTypeCode.Supplier)]
    public void CreateAccount_WithSupplierType_CreatesCreditAssessmentTask(Account_CustomerTypeCode customerType)
    {
        // Arrange & Act
        var account = Producer.ProduceValidAccount(new Account { CustomerTypeCode = customerType });

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == account.Id));
        tasks.Should().HaveCount(1, "A credit assessment task should be created for a Supplier account.");
    }

    [Theory]
    [InlineData(Account_CustomerTypeCode.Customer)]
    [InlineData(Account_CustomerTypeCode.Partner)]
    public void CreateAccount_WithNonSupplierType_DoesNotCreateCreditAssessmentTask(Account_CustomerTypeCode customerType)
    {
        // Arrange & Act
        var account = Producer.ProduceValidAccount(new Account { CustomerTypeCode = customerType });

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == account.Id));
        tasks.Should().BeEmpty("No credit assessment task should be created for a non-Supplier account.");
    }

    [Fact]
    public void CreateAccount_WithNullCustomerType_DoesNotCreateCreditAssessmentTask()
    {
        // Arrange & Act
        var account = Producer.ProduceValidAccount(new Account { CustomerTypeCode = null });

        // Assert
        var tasks = AdminDao.RetrieveList(xrm => xrm.TaskSet.Where(t => t.RegardingObjectId.Id == account.Id));
        tasks.Should().BeEmpty("No credit assessment task should be created for an account with null Customer Type.");
    }
}