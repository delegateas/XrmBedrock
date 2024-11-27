using Microsoft.Extensions.Logging;

namespace LF.Medlemssystem.PluginTests;

public class ConsoleLogger : ILogger
{
    public ConsoleLogger()
    {
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");
        Console.WriteLine($"{formatter(state, exception)}");
    }
}