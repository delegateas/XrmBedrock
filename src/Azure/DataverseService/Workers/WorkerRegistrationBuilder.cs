using Microsoft.Extensions.DependencyInjection;

namespace DataverseService.Workers;

public sealed class WorkerRegistrationBuilder(IServiceCollection services, string storageAccountEnvironmentVariableName)
{
    public WorkerRegistrationBuilder AddWorker<TWorker>()
        where TWorker : class
    {
        var workerType = typeof(TWorker);
        var messageType = WorkerTypeInspector.GetMessageType(workerType);
        var queueName = WorkerTypeInspector.GetQueueName(workerType);

        services.AddQueueWorkerInternal(workerType, messageType, queueName, _ =>
        {
            var storageAccountUrl = Environment.GetEnvironmentVariable(storageAccountEnvironmentVariableName)
                ?? throw new InvalidOperationException($"Environment variable '{storageAccountEnvironmentVariableName}' is not set.");

            return new Uri($"{storageAccountUrl.TrimEnd('/')}/{queueName}");
        });

        return this;
    }
}
