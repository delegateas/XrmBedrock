namespace DataverseServiceTests;

/// <summary>
/// This is more of a demo of the issue of trying to make centralized and uniform error handling in an Async setup
/// There will be implemented two sets of Async-methods in the Async extension of the DAO. One that is awaited and therefore can have error handling like the sync-version and one will be unawaited to support paralisation
/// </summary>
public class TestExceptionHandlingAwaitedAndUnawaited
{
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