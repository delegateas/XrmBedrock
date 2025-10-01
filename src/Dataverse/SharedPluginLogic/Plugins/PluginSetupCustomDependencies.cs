using DataverseLogic;
using DataverseLogic.Azure;
using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;

namespace Dataverse.Plugins;

internal static class PluginSetupCustomDependencies
{
    internal static void SetupCustomDependencies(this ServiceCollection services)
    {
        services.AddAzureConfig();

        // Utility
        services.AddScoped<IDuplicateRuleService, DuplicateRuleService>();

        // Dao objects
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IUserDataverseAccessObjectService, UserDataverseAccessObjectService>();

        // Integration logic (lexicografical order please)
        services.AddScoped<IAzureService, AzureService>();

        // Dataverse Logic (lexicografical order please)
        // Add your custom service registrations here
    }
}