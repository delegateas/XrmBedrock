using DataverseService;
using DataverseService.ActivityArea;
using DataverseService.Foundation.Dao;
using DataverseService.UtilityArea;
using DataverseService.UtilityArea.DataMigration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContext.Dao;
using SharedDataverseLogic.ActivityArea;
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
        services.AddDataverse(enableAffinityCookie: false); // affinity cookie is disabled to optimize parallelism for DataMigration Function

        // Add required services
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IDataverseActivityService, DataverseActivityService>();
        services.AddScoped<IDataverseMigrateGenericService, DataverseMigrateGenericService>();
        services.AddScoped<IDataverseOrganizationRequestService, DataverseOrganizationRequestService>();
        services.AddScoped<ISharedDataverseActivityService, SharedDataverseActivityService>();

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