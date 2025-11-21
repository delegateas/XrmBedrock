using Microsoft.Extensions.DependencyInjection;

namespace DataverseService.EconomyArea;

public static class AddServices
{
    public static void AddEconomyServices(this IServiceCollection services)
    {
        services.AddScoped<DataverseInvoiceService>();
    }
}
