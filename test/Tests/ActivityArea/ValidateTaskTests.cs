using Microsoft.Xrm.Sdk;
using Task = XrmBedrock.SharedContext.Task;

namespace Tests.ActivityArea;

public class ValidateTaskTests(XrmMockupFixture fixture) : TestBase(fixture)
{
    [Fact]
    public void TestNoValidationOfOwnTask_Implicit()
    {
        // Arrange
        var account = Producer.ProduceValidAccount(null);
        var userProducer = new DataProducer(UserDao, AdminDao);

        // Act
        var act = () => userProducer.ProduceValidTask(new Task
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
        var userProducer = new DataProducer(UserDao, AdminDao);

        // Act
        var act = () => userProducer.ProduceValidTask(new Task
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
        var userProducer = new DataProducer(UserDao, AdminDao);

        // Act
        var act = () => userProducer.ProduceValidTask(new Task
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
        var userProducer = new DataProducer(UserDao, AdminDao);
        var task = userProducer.ProduceValidTask(new Task
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
