using Microsoft.Extensions.DependencyInjection;

namespace DataverseLogic.EconomyArea;

public static class AddServices
{
    public static void AddEconomyArea(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceCollectionService, InvoiceCollectionService>();
        services.AddScoped<ITransactionService, TransactionService>();
    }
}