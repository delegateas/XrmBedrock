using Microsoft.Xrm.Sdk;
using Task = XrmBedrock.SharedContext.Task;

namespace IntegrationTests.ActivityArea;

public class ValidateTaskTests : TestBase
{
    public ValidateTaskTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void TestNoValidationOfOwnTask_Implicit()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);

        // Act
        var act = () => UserDao.Producer(new Task
        {
            RegardingObjectId = account.ToEntityReference(),
            ScheduledStart = new DateTime(2025, 1, 1, 3, 0, 0, DateTimeKind.Utc),
        });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TestBlockOutsideBusinessHours()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);

        // Act
        var act = () => UserDao.Producer(new Task
        {
            RegardingObjectId = account.ToEntityReference(),
            ScheduledStart = new DateTime(2025, 1, 1, 3, 0, 0, DateTimeKind.Utc),
            OwnerId = OtherUserReference,
        });

        // Assert
        act.Should().Throw<InvalidPluginExecutionException>();
    }

    [Fact]
    public void TestNoBlockWithinBusinessHours()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);

        // Act
        var act = () => UserDao.Producer(new Task
        {
            RegardingObjectId = account.ToEntityReference(),
            ScheduledStart = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            OwnerId = OtherUserReference,
        });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TestBlockOnSwapOwnerOfOutsideTask()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var task = UserDao.Producer(new Task
        {
            RegardingObjectId = account.ToEntityReference(),
            ScheduledStart = new DateTime(2025, 1, 1, 3, 0, 0, DateTimeKind.Utc),
        });

        // Act
        var act = () => UserDao.Update(new Task(task.Id)
        {
            OwnerId = OtherUserReference,
        });

        // Assert
        act.Should().Throw<InvalidPluginExecutionException>();
    }
}