using DataverseService.Dto.Activity;
using SharedContext.Dao;
using SharedDataverseLogic.ActivityArea;
using SharedDomain;
using XrmBedrock.SharedContext;
using DataverseTask = XrmBedrock.SharedContext.Task;

namespace DemoExternalApi.BusinessLogic;

public class ActivityBusinessLogic : IActivityBusinessLogic
{
    private readonly ISharedDataverseActivityService sharedDataverseActivityService;
    private readonly ILoggingComponent logger;

    public ActivityBusinessLogic(
        ISharedDataverseActivityService sharedDataverseActivityService,
        ILoggingComponent logger)
    {
        this.sharedDataverseActivityService = sharedDataverseActivityService;
        this.logger = logger;
    }

    public CreateTaskResponse CreateTask(CreateTaskRequest createTaskRequest)
    {
        ArgumentNullException.ThrowIfNull(createTaskRequest);
        logger.LogInformation("Creating Task Request in Dataverse");

        var task = new DataverseTask
        {
            Subject = createTaskRequest.Subject,
            Description = createTaskRequest.Description,
            ScheduledEnd = DateTime.Now.AddDays(4),
            RegardingObjectId = true ? createTaskRequest.ContactOrAccountId.ToEntityReference<Contact>() : createTaskRequest.ContactOrAccountId.ToEntityReference<Account>(),
        };

        return new CreateTaskResponse(sharedDataverseActivityService.CreateTaskInQueue(task, createTaskRequest.QueueId));
    }
}