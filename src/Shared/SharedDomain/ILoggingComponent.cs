using Microsoft.Extensions.Logging;

namespace SharedDomain;

public interface ILoggingComponent
{
    void SetLogger(ILogger? logger);

    void LogInformation(string message);

    void LogError(string message);

    void LogError(Exception exception, string message);

    void LogWarning(string message);
}