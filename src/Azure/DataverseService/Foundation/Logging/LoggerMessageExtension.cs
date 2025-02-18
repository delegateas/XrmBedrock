using Microsoft.Extensions.Logging;

namespace DataverseService.Foundation.Logging;

public static class LoggerMessageExtension
{
    private static readonly Action<ILogger, string, System.Exception?> LoggerMessageInformation =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            0,
            "LogInformation : {Message}");

    public static void LogMessageInformation(this ILogger logger, string message)
    {
        LoggerMessageInformation(logger, message, null);
    }
}