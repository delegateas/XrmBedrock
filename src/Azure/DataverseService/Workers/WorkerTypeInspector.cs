using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DataverseService.Workers;

internal static class WorkerTypeInspector
{
    public static Type GetMessageType(Type workerType)
    {
        var baseType = workerType.BaseType;

        while (baseType is not null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(QueueWorker<>))
                return baseType.GetGenericArguments()[0];

            baseType = baseType.BaseType;
        }

        throw new InvalidOperationException(
            $"Worker type '{workerType.Name}' must inherit from QueueWorker<TMessage>.");
    }

    public static string GetQueueName(Type workerType)
    {
        var constructors = workerType.GetConstructors();

        foreach (var constructor in constructors)
        {
            foreach (var parameter in constructor.GetParameters())
            {
                var fromKeyedServicesAttribute = parameter.GetCustomAttribute<FromKeyedServicesAttribute>();

                if (fromKeyedServicesAttribute?.Key is string queueName)
                    return queueName;
            }
        }

        throw new InvalidOperationException(
            $"Worker type '{workerType.Name}' must have a constructor parameter with [FromKeyedServices(queueName)] attribute.");
    }
}
