using Task = System.Threading.Tasks.Task;

namespace DemoExternalApi.Tests;

public class ActivityBusinessLogicTests : TestBase
{
    public ActivityBusinessLogicTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public Task TestAwaited()
    {
        return Assert.ThrowsAsync<InvalidOperationException>(() => AwaitedHandling(FailWithDelayMethod));
    }

    [Fact]
    public Task TestUnAwaited()
    {
        return Assert.ThrowsAsync<NotImplementedException>(() => UnAwaitedHandling(FailWithDelayMethod));
    }

    private async Task<int> FailWithDelayMethod()
    {
        await Task.Delay(3000);
        throw new NotImplementedException();
    }

    private static async Task<int> AwaitedHandling(Func<Task<int>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.ToString());
        }
    }

    private static Task<int> UnAwaitedHandling(Func<Task<int>> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.ToString());
        }
    }
}