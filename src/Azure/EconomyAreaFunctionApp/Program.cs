using Azure.Identity;
using DataverseService;
using DataverseService.EconomyArea;
using DataverseService.Foundation.Dao;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContext.Dao;

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
        services.AddDataverse(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? new AzureCliCredential()
                : new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")));

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

        services.AddEconomyServices();
    })
    .Build();

host.Run();