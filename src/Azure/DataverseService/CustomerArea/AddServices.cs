using Microsoft.Extensions.DependencyInjection;

namespace DataverseService.CustomerArea;

public static class AddServices
{
    public static void AddCustomerServices(this IServiceCollection services)
    {
        services.AddScoped<DataverseSubscriptionService>();
    }
}
