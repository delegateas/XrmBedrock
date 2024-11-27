namespace DataverseService.Dto.Activity;

public record CreateTaskRequest(Guid ContactOrAccountId, Guid QueueId, string Subject, string Description, string Message)
{
}