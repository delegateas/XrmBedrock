using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;

namespace DataverseLogic.Azure;

public static class AzureConfigSetter
{
    public static void AddAzureConfig(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var dao = provider.GetRequiredService<IAdminDataverseAccessObjectService>();

            var urlFromEnvVar = dao.RetrieveFirst(xrm =>
                from variable in xrm.EnvironmentVariableValueSet
                join definition in xrm.EnvironmentVariableDefinitionSet on variable.EnvironmentVariableDefinitionId.Id equals definition.Id
                where definition.SchemaName == "mgs_AzureStorageAccountUrl"
                select variable.Value);

            var storageAccountUrl = urlFromEnvVar ?? "https://www.microsoft.com/";

            return new AzureConfig(
                new Uri(storageAccountUrl));
        });
    }
}