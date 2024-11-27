using DataverseService;
using DataverseService.Foundation.Dao;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContext.Dao;
using SharedDomain;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.AddScoped<ILoggingComponent, LoggingComponent>();
        services.AddDataverse();

        // Add required services
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.Configure<LoggerFilterOptions>(options =>
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (toRemove is not null)
            {
                options.Rules.Remove(toRemove);
            }
        });
    })
    .Build();

host.Run();

// So stylecop rules does not allow a blank line at the end of the file but VS automatically adds it when saving the file unless last line is a } or a comment *sigh*