namespace Dataverse.PluginTests.ActivityArea;

public class DemoValidateTaskTest : TestBase
{
    public DemoValidateTaskTest(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void TestNoValidationOfOwnTask_Implicit()
    {
        // Arrange
        // Act
        var act = () => Producer.ProduceValidAccount(null);

        // Assert
        act.Should().NotThrow();
    }
}