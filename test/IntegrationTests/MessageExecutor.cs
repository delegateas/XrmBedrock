using DataverseService.Workers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace IntegrationTests;

/// <summary>
/// Enables the storage of serialized messages to a storage queue.
/// Connects queue messages to the correct service using auto-discovery from WorkerRegistrations.
/// </summary>
public sealed class MessageExecutor
{
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<string, MessageHandlerInfo> handlerMapping;
    private List<AwaitingMessage> messages;

    public MessageExecutor(IServiceProvider serviceProvider, IEnumerable<WorkerRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(registrations);

        this.serviceProvider = serviceProvider;
        messages = [];
        handlerMapping = BuildHandlerMapping(registrations);
    }

    private static Dictionary<string, MessageHandlerInfo> BuildHandlerMapping(IEnumerable<WorkerRegistration> registrations)
    {
        var mapping = new Dictionary<string, MessageHandlerInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in registrations)
        {
            mapping[registration.QueueName] = new MessageHandlerInfo(registration.MessageType, registration.WorkerType);
        }

        return mapping;
    }

    public void StoreMessage(AwaitingMessage message) => messages.Add(message);

    public async Task SendMessages()
    {
        foreach (var message in messages)
        {
            if (!handlerMapping.TryGetValue(message.QueueName, out var handlerInfo))
                throw new InvalidOperationException($"No handler registered for queue '{message.QueueName}'.");

            using var scope = serviceProvider.CreateScope();

            var payload = DeserializeMessage(message.SerializedMessage, handlerInfo.MessageType);

            await InvokeProcessMessageAsync(handlerInfo.WorkerType, payload, scope.ServiceProvider);
        }

        messages = [];
    }

    private static async Task InvokeProcessMessageAsync(Type workerType, object message, IServiceProvider scopedServices)
    {
        var processMethod = workerType.GetMethod(
            "ProcessMessageAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        if (processMethod is null)
            throw new InvalidOperationException($"Worker type {workerType.Name} does not have a ProcessMessageAsync method.");

        var messageType = message.GetType();
        var serviceInfo = ResolveServiceFromMessageType(messageType);

        if (serviceInfo is null)
            throw new InvalidOperationException($"Cannot resolve service for message type {messageType.Name}.");

        var service = scopedServices.GetRequiredService(serviceInfo.ServiceType);
        var method = serviceInfo.ServiceType.GetMethod(serviceInfo.MethodName);

        if (method is null)
            throw new InvalidOperationException($"Method {serviceInfo.MethodName} not found on {serviceInfo.ServiceType.Name}.");

        var parameters = ExtractMethodParameters(message, method);
        var result = method.Invoke(service, parameters);

        if (result is Task task)
        {
            await task;
        }
    }

    private static ServiceResolutionInfo? ResolveServiceFromMessageType(Type messageType)
    {
        var messageName = messageType.Name;

        if (!messageName.EndsWith("Message", StringComparison.Ordinal))
            return null;

        var actionAndDomain = messageName[..^7];

        var dataverseServiceAssembly = Array.Find(
            AppDomain.CurrentDomain.GetAssemblies(),
            a => a.GetName().Name == "DataverseService");

        if (dataverseServiceAssembly is null)
            return null;

        var candidateTypes = dataverseServiceAssembly.GetTypes()
            .Where(t => t.Name.StartsWith("Dataverse", StringComparison.Ordinal) && t.Name.EndsWith("Service", StringComparison.Ordinal));

        foreach (var serviceType in candidateTypes)
        {
            var method = serviceType.GetMethod(actionAndDomain);
            if (method is not null)
                return new ServiceResolutionInfo(serviceType, actionAndDomain);
        }

        return null;
    }

    private static object?[] ExtractMethodParameters(object message, System.Reflection.MethodInfo method)
    {
        var methodParams = method.GetParameters();
        var messageType = message.GetType();
        var result = new object?[methodParams.Length];

        // For record types, properties are declared in constructor parameter order
        // Match by position since message property names may differ from method parameter names
        var properties = messageType.GetProperties()
            .Where(p => p.CanRead && p.GetMethod?.IsPublic == true)
            .ToArray();

        for (var i = 0; i < methodParams.Length && i < properties.Length; i++)
        {
            result[i] = properties[i].GetValue(message);
        }

        return result;
    }

    private static object DeserializeMessage(string serializedMessage, Type messageType)
    {
        ArgumentNullException.ThrowIfNull(serializedMessage);

        var message = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    serializedMessage
                        .Replace("<QueueMessage><MessageText>", string.Empty, true, CultureInfo.InvariantCulture)
                        .Replace("</MessageText></QueueMessage>", string.Empty, true, CultureInfo.InvariantCulture))),
            messageType);

        return message ?? throw new InvalidOperationException($"Failed to deserialize message to type {messageType.Name}.");
    }

    private sealed record MessageHandlerInfo(Type MessageType, Type WorkerType);

    private sealed record ServiceResolutionInfo(Type ServiceType, string MethodName);
}
