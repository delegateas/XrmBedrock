using Microsoft.Extensions.Logging;
using SharedDataverseLogic;

namespace DataverseService.Foundation.Logging;

/// <summary>
/// This class is here to support tracing in services in the SharedDataverseLogic project
/// </summary>
public class ExtendedTracingService : IExtendedTracingService
{
    private readonly ILogger logger;

    public ExtendedTracingService(ILogger<ExtendedTracingService> logger)
    {
        this.logger = logger;
    }

    public void Trace(string format, params object[] args)
    {
        logger?.LogTrace(format, args);
    }
}