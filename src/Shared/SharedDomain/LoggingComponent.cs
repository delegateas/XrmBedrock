using Microsoft.Extensions.Logging;

namespace SharedDomain;

public class LoggingComponent : ILoggingComponent
{
    private ILogger? logger;

    public void SetLogger(ILogger? logger)
    {
        this.logger = logger;
    }

    public void LogInformation(string message)
    {
        logger?.LogInformation("{Message}", message);
    }

    public void LogError(string message)
    {
        logger?.LogError("{Message}", message);
    }

    public void LogError(Exception exception, string message)
    {
        if (exception != null)
        {
            logger?.LogError(exception, "{Message}", message);
        }
        else
        {
            logger?.LogError("{Message}", message);
        }
    }

    public void LogWarning(string message)
    {
        logger?.LogWarning("{Message}", message);
    }
}