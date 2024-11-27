namespace SharedDataverseLogic.ActivityArea;

public interface ISharedDataverseActivityService
{
    Guid CreateTaskInQueue(XrmBedrock.SharedContext.Task task, Guid queueId);
}