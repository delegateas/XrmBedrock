namespace DataverseService.Workers;

public sealed record WorkerRegistration(Type WorkerType, Type MessageType, string QueueName);
