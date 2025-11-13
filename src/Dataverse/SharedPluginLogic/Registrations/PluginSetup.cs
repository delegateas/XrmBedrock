using DataverseLogic;
using DataverseLogic.Azure;
using DataverseLogic.EconomyArea;
using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using System.Globalization;

namespace DataverseRegistration;

internal static class PluginSetup
{
    internal static void SetupCustomDependencies(this ServiceCollection services)
    {
        services.AddAzureConfig();

        // Utility
        services.AddScoped<DuplicateRuleService>();

        // Dao objects
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IUserDataverseAccessObjectService, UserDataverseAccessObjectService>();

        // Integration logic (lexicografical order please)
        services.AddScoped<AzureService>();

        // Dataverse Logic (lexicografical order please)
        // Add your custom service registrations here
        services.AddEconomyArea();
    }

    internal static ServiceProvider BuildServiceProvider(this IServiceProvider serviceProvider, string className)
    {
        // Get services of the ServiceProvider
        var tracingService = serviceProvider.GetService<ITracingService>() ?? throw new InvalidPluginExecutionException("Unable to get Tracing service");
        var pluginTelemetryLogger = serviceProvider.GetService<Microsoft.Xrm.Sdk.PluginTelemetry.ILogger>();
        if (pluginTelemetryLogger == null)
        {
            tracingService.Trace("Unable to get PluginTelemetryLogger");
        }

        var extendedTracingService = new ExtendedTracingService(tracingService, pluginTelemetryLogger!);
        extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", className));
        var context = serviceProvider.GetService<IPluginExecutionContext>() ?? throw new InvalidPluginExecutionException("Unable to get PluginBase Execution Context");
        var organizationServiceFactory = serviceProvider.GetService<IOrganizationServiceFactory>() ?? throw new InvalidPluginExecutionException("Unable to get service factory");
        var managedIdentity = serviceProvider.GetService<IManagedIdentityService>() ?? new DummyManagedIdentityService();

        // Create a new service collection and add the relevant services
        var services = new ServiceCollection();
        services.AddScoped(x => context);
        services.AddScoped(x => tracingService);
        services.AddScoped<ITracingService>(x => extendedTracingService);
        services.AddScoped(x => organizationServiceFactory);
        services.AddScoped(x => managedIdentity);
        services.AddScoped<ILogger, DataverseLogger>();
        services.TryAdd(ServiceDescriptor.Scoped(typeof(ILogger<>), typeof(DataverseLogger<>)));

        services.SetupCustomDependencies();

        return services.BuildServiceProvider();
    }
}