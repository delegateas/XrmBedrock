using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataverseService.Workers;

public static class WorkerRegistrationExtensions
{
    internal static IServiceCollection AddQueueWorkerInternal(
        this IServiceCollection services,
        Type workerType,
        Type messageType,
        string queueName,
        Func<IServiceProvider, Uri> queueUriFactory)
    {
        services.AddKeyedSingleton(queueName, (provider, _) =>
        {
            var queueUri = queueUriFactory(provider);
            return new QueueClient(queueUri, new DefaultAzureCredential());
        });

        services.AddSingleton(new WorkerRegistration(workerType, messageType, queueName));

        services.AddSingleton(typeof(IHostedService), workerType);

        return services;
    }

    public static IServiceCollection AddAreaWorkers(
        this IServiceCollection services,
        string storageAccountEnvironmentVariableName,
        Action<WorkerRegistrationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new WorkerRegistrationBuilder(services, storageAccountEnvironmentVariableName);
        configure(builder);
        return services;
    }
}
