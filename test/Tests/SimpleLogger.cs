using Microsoft.Extensions.Logging;

namespace SharedTest;

public class SimpleLogger : ILogger
{
#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public IDisposable? BeginScope<TState>(TState state)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
        where TState : notnull
        => null; // No-op

    public bool IsEnabled(LogLevel logLevel) => true; // Always enabled

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        // Simple log output
        if (exception != null && formatter != null)
        {
            Console.WriteLine($"{DateTime.Now}: {formatter(state, exception)}");
        }

        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }
}

#pragma warning disable SA1402 // File may only contain a single type
public class SimpleLogger<T> : ILogger<T>
#pragma warning restore SA1402 // File may only contain a single type
{
    private readonly SimpleLogger logger = new SimpleLogger();

#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public IDisposable? BeginScope<TState>(TState state)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
        where TState : notnull
        => logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
