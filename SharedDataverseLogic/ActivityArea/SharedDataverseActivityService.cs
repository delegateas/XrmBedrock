using Microsoft.Crm.Sdk.Messages;
using SharedContext.Dao;
using DataverseTask = XrmBedrock.SharedContext.Task;

namespace SharedDataverseLogic.ActivityArea;

public class SharedDataverseActivityService : ISharedDataverseActivityService
{
    private readonly IAdminDataverseAccessObjectService adminDao;

    public SharedDataverseActivityService(IAdminDataverseAccessObjectService adminDao)
    {
        this.adminDao = adminDao;
    }

    public Guid CreateTaskInQueue(DataverseTask task, Guid queueId)
    {
        var taskId = CreateTask(task);
        AddTaskToQueue(taskId, queueId);
        return taskId;
    }

    private Guid CreateTask(DataverseTask task)
    {
        return adminDao.Create(task);
    }

    private void AddTaskToQueue(Guid taskId, Guid queueId)
    {
        adminDao.Execute(new AddToQueueRequest
        {
            Target = taskId.ToEntityReference<DataverseTask>(),
            DestinationQueueId = queueId,
        });
    }
}