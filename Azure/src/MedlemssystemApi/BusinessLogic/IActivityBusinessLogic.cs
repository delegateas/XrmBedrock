using DataverseService.Dto.Activity;

namespace MedlemssystemApi.BusinessLogic;

public interface IActivityBusinessLogic
{
    CreateTaskResponse CreateTask(CreateTaskRequest createTaskRequest);
}