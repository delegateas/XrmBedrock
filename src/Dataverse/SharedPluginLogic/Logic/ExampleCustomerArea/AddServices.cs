using Microsoft.Extensions.DependencyInjection;

namespace Dataverse.PluginLogic.ExampleCustomerArea;

public static class AddServices
{
    // Add all services for the Example Activity Area
    public static void AddExampleCustomerArea(this IServiceCollection services)
    {
        // Missing interfaces here?
        // We actually do not need these for the services that implement the actual plugin logic as this IS exactly the logic we want to test,
        // so exchanging the implementation for _these_ services does no make sense
        services.AddScoped<ExampleCustomerService>();
    }
}