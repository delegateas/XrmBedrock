using Dataverse.PluginLogic.ExampleActivityArea;
using DataverseLogic;
using DataverseLogic.Azure;
using DataverseLogic.EconomyArea;
using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;
using SharedDomain;

namespace Dataverse.Plugins;

internal static class PluginSetupCustomDependencies
{
#pragma warning disable MA0051 // Method is too long
    internal static void SetupCustomDependencies(this ServiceCollection services)
#pragma warning restore MA0051 // Method is too long
    {
        services.AddAzureConfig();

        // Utility
        services.AddScoped<IDuplicateRuleService, DuplicateRuleService>();
        services.AddScoped<ILoggingComponent, LoggingComponent>();

        // Dao objects
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IUserDataverseAccessObjectService, UserDataverseAccessObjectService>();

        // Integration logic (lexicografical order please)
        services.AddScoped<IAzureService, AzureService>();

        // Dataverse Logic (lexicografical order please)
        services.AddEconomyArea();
        services.AddExampleActivityArea();
    }
}