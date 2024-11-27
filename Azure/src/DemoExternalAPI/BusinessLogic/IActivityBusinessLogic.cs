using DataverseService.Dto.Activity;

namespace DemoExternalApi.BusinessLogic;

public interface IActivityBusinessLogic
{
    CreateTaskResponse CreateTask(CreateTaskRequest createTaskRequest);
}