using Newtonsoft.Json;
using SharedDomain;
using SharedDomain.CustomerArea;
using System.Text;
using XrmBedrock.SharedContext;

namespace Dataverse.PluginTests.CustomerArea;

public class DemoAddMessageToAzureQueueTests : TestBase
{
    public DemoAddMessageToAzureQueueTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void TestQueuedOnCreateWithPrimaryContact()
    {
        // Arrange
        var contact = Producer.ProduceValidContact(null);
        var account = Producer.ConstructValidAccount(new Account() { PrimaryContactId = contact.ToEntityReference() });

        // Act
        var accountId = AdminDao.Create(account);

        // Assert
        AssertQueued(accountId);
    }

    [Fact]
    public void TestNotQueuedOnCreateWithoutPrimaryContact()
    {
        // Arrange
        var account = Producer.ConstructValidAccount(new Account() { });

        // Act
        AdminDao.Create(account);

        // Assert
        AssertNoneQueued();
    }

    [Fact]
    public void TestQueuedOnUpdateWithPrimaryContact()
    {
        // Arrange
        var contact = Producer.ProduceValidContact(null);
        var account = Producer.ProduceValidAccount(null);

        // Act
        AdminDao.Update(new Account(account.Id) { PrimaryContactId = contact.ToEntityReference() });

        // Assert
        AssertQueued(account.Id);
    }

    [Fact]
    public void TestQueuedAgainOnChangeOfPrimaryContact()
    {
        // Arrange
        var contact1 = Producer.ProduceValidContact(null);
        var contact2 = Producer.ProduceValidContact(null);
        var account = Producer.ProduceValidAccount(null);

        // Act
        AdminDao.Update(new Account(account.Id) { PrimaryContactId = contact1.ToEntityReference() });
        AdminDao.Update(new Account(account.Id) { PrimaryContactId = contact2.ToEntityReference() });

        // Assert
        AssertQueued(account.Id, 2);
    }

    [Fact]
    public void TestNotRequedOnTouchOfPrimaryContact()
    {
        // Arrange
        var contact = Producer.ProduceValidContact(null);
        var account = Producer.ProduceValidAccount(null);

        // Act
        AdminDao.Update(new Account(account.Id) { PrimaryContactId = contact.ToEntityReference() });
        AdminDao.Update(new Account(account.Id) { PrimaryContactId = contact.ToEntityReference() });

        // Assert
        AssertQueued(account.Id, 1);
    }

    /// <summary>
    /// The queueing is mocked so we cannot actually test that the message is in the queue but we simulate by looking at the log entries
    /// </summary>
    private void AssertQueued(Guid accountId, int count = 1)
    {
        LogEntries.Count(e => e.RequestMessage.Path.Contains(QueueNames.DemoAccountQueue)).Should().Be(count);

        var body =
            LogEntries
            .Last(e => e.RequestMessage.Path.Contains(QueueNames.DemoAccountQueue)).RequestMessage
            .Body;

        body
        .Should()
        .NotBeNull()
        .And
        .StartWith("<QueueMessage><MessageText>")
        .And
        .EndWith("</MessageText></QueueMessage>");

        var message =
            JsonConvert.DeserializeObject<DemoAccountMessage>(
               Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        body?
                        .Replace("<QueueMessage><MessageText>", "")
                        .Replace("</MessageText></QueueMessage>", ""))));

        message.Should().NotBeNull();
        message?.AccountId.Should().Be(accountId);
    }

    private void AssertNoneQueued()
    {
        LogEntries.Count(e => e.RequestMessage.Path.Contains(QueueNames.DemoAccountQueue)).Should().Be(0);
    }
}