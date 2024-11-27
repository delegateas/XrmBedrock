using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;

namespace DataverseLogic.Azure;

public static class AzureConfigSetter
{
    public static void AddAzureConfig(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var dao = provider.GetRequiredService<IDataverseAccessObject>();

            // TODO: Check that this query is correct
            var urlFromEnvVar = dao.RetrieveFirst(xrm => xrm.EnvironmentVariableValueSet
                .Where(ev => ev.EnvironmentVariableDefinitionId.Name == "AzureStorageAccountUrl")
                .Select(ev => ev.Value));

            var storageAccountUrl = urlFromEnvVar ?? "https://www.microsoft.com/";

            return new AzureConfig(
                new Uri(storageAccountUrl));
        });
    }
}