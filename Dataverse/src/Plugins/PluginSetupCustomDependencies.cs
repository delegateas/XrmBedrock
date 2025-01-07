using DataverseLogic.ActivityArea;
using DataverseLogic.Azure;
using DataverseLogic.CustomerArea;
using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;
using SharedDataverseLogic.ActivityArea;
using SharedDomain;

namespace Dataverse.Plugins;

internal static class PluginSetupCustomDependencies
{
#pragma warning disable MA0051 // Method is too long
    internal static void SetupCustomDependencies(this ServiceCollection services)
#pragma warning restore MA0051 // Method is too long
    {
        services.AddAzureConfig();

        // Integration logic (lexicografical order please)
        services.AddScoped<IAzureService, AzureService>();

        // Dataverse Logic (lexicografical order please)
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDuplicateRuleService, DuplicateRuleService>();
        services.AddScoped<ILoggingComponent, LoggingComponent>();
        services.AddScoped<ISharedDataverseActivityService, SharedDataverseActivityService>();
    }
}