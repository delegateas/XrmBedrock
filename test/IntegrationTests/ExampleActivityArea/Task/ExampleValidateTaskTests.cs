using DG.Tools.XrmMockup;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using XrmBedrock.SharedContext;
using Task = XrmBedrock.SharedContext.Task;

namespace IntegrationTests.ExampleActivityArea;

public class ExampleValidateTaskTests : TestBase
{
    private readonly Guid someUserId;

    public ExampleValidateTaskTests(XrmMockupFixture fixture)
        : base(fixture)
    {
        someUserId = CreateUser(Guid.NewGuid(), Xrm.RootBusinessUnit, SecurityRoles.SystemAdministrator).Id;
    }

    [Fact]
    public void TestNoValidationOfOwnTask_Implicit()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = Producer.ConstructValidTask(new Task()
        {
            ScheduledStart = DateTime.Now.Date.AddHours(3),
            RegardingObjectId = account.ToEntityReference(),
        });

        // Act
        var act = () => UserDao.Create(task);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TestNoValidationOfOwnTask_Explicit()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = Producer.ConstructValidTask(new Task()
        {
            ScheduledStart = DateTime.Now.Date.AddHours(3),
            RegardingObjectId = account.ToEntityReference(),
            OwnerId = UserIdOfUserDao.ToEntityReference<SystemUser>(),
        });

        // Act
        var act = () => UserDao.Create(task);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TestBlockOutsideBusinessHours()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = Producer.ConstructValidTask(new Task()
        {
            ScheduledStart = DateTime.Now.Date.AddHours(3),
            RegardingObjectId = account.ToEntityReference(),
            OwnerId = someUserId.ToEntityReference<SystemUser>(),
        });

        // Act
        var act = () => UserDao.Create(task);

        // Assert
        act.Should().Throw<InvalidPluginExecutionException>();
    }

    [Fact]
    public void TestNoBlockWithinBusinessHours()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = Producer.ConstructValidTask(new Task()
        {
            ScheduledStart = DateTime.Now.Date.AddHours(10),
            RegardingObjectId = account.ToEntityReference(),
            OwnerId = someUserId.ToEntityReference<SystemUser>(),
        });

        // Act
        var act = () => UserDao.Create(task);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TestBlockOnSwapOwnerOfOutsideTask()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = Producer.ConstructValidTask(new Task()
        {
            ScheduledStart = DateTime.Now.Date.AddHours(3),
            RegardingObjectId = account.ToEntityReference(),
        });
        var taskId = UserDao.Create(task);

        // Act
        var act = () => UserDao.Update(new Task(taskId) { OwnerId = someUserId.ToEntityReference<SystemUser>(), });

        // Assert
        act.Should().Throw<InvalidPluginExecutionException>();
    }
}