using Azure.Core;
using Azure.DataverseService.Foundation.Dao;
using DataverseConnection;
using DataverseService.Foundation.Dao;
using DataverseService.Foundation.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace DataverseService;

public static class Startup
{
    public static IServiceCollection AddDataverse(this IServiceCollection services, TokenCredential tokenCredential)
    {
        services.AddScoped(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("DataverseService"));
        services.AddDataverseWithOrganizationServices(options =>
        {
            options.TokenCredential = tokenCredential;
        });
        services.AddScoped<IDataverseAccessObjectAsync, DataverseAccessObjectAsync>();
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<ITracingService, ExtendedTracingService>();

        return services;
    }
}