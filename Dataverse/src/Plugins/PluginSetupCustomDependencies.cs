using DataverseLogic;
using DataverseLogic.Azure;
using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;
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

        // Dao objects
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IUserDataverseAccessObjectService, UserDataverseAccessObjectService>();

        // Integration logic (lexicografical order please)
        services.AddScoped<IAzureService, AzureService>();

        // Dataverse Logic (lexicografical order please)
        //services.AddScoped<IAccountService, AccountService>();
        //services.AddScoped<IActivityService, ActivityService>();
        //services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IDuplicateRuleService, DuplicateRuleService>();
        services.AddScoped<ILoggingComponent, LoggingComponent>();
        services.AddScoped<ISharedDataverseActivityService, SharedDataverseActivityService>();
    }
}