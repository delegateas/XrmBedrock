using Azure.DataverseService.Foundation.Dao;
using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace IntegrationTests.Infrastructure;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection ReplaceDataverseServices(
        this IServiceCollection services,
        XrmMockup365 xrm)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(xrm);

        // Remove production Dataverse registrations (TokenCredential-based)
        var descriptorsToRemove = services
            .Where(d => d.ServiceType == typeof(IDataverseAccessObjectAsync) ||
                        d.ServiceType == typeof(IOrganizationServiceAsync2) ||
                        d.ServiceType == typeof(ServiceClient))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
            services.Remove(descriptor);

        // Register ILogger for DataverseAccessObjectAsync (non-generic ILogger is required)
        services.AddScoped<ILogger>(sp =>
            sp.GetRequiredService<ILoggerFactory>().CreateLogger("DataverseAccessObjectAsync"));

        // Register XrmMockup's async organization service
        services.AddScoped<IOrganizationServiceAsync2>(_ => xrm.GetAdminService());

        // Register actual DataverseAccessObjectAsync using XrmMockup's service
        services.AddScoped<IDataverseAccessObjectAsync, DataverseAccessObjectAsync>();

        return services;
    }
}
