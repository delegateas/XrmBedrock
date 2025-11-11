using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace DataverseRegistration;

public class DataverseLogger : ILogger
{
    private readonly ITracingService service;

    public DataverseLogger(ITracingService service)
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        this.service = service;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => service != null;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));

        service.Trace($"[{eventId.Id,2}: {logLevel,-12}]");
        service.Trace($"{formatter(state, exception)}");
    }
}

#pragma warning disable SA1402 // File may only contain a single type
public class DataverseLogger<T> : ILogger<T>
#pragma warning restore SA1402 // File may only contain a single type
{
    private readonly ITracingService service;

    public DataverseLogger(ITracingService service)
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        this.service = service;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => service != null;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));

        service.Trace($"[{eventId.Id,2}: {logLevel,-12}]");
        service.Trace($"{formatter(state, exception)}");
    }
}