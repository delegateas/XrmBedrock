using DataverseService.Dto.Activity;
using DataverseService.Foundation.Logging;
using DemoExternalApi.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DemoExternalApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityBusinessLogic activityBusinessLogic;
    private readonly ILogger<ActivityController> logger;

    public ActivityController(IActivityBusinessLogic activityBusinessLogic, ILogger<ActivityController> logger)
    {
        this.activityBusinessLogic = activityBusinessLogic;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="createTaskRequest">The task to create.</param>
    /// <returns>The ID of the created task as a Guid.</returns>
    /// <response code="201">Registered successfully and returns the new tasks ID.</response>
    /// <response code="400">If the item is null.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("createTask")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<CreateTaskResponse> CreateTask(CreateTaskRequest createTaskRequest)
    {
        if (createTaskRequest == null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "createTaskRequest is null.",
            });
        }

        try
        {
            logger.LogMessageInformation($"CreateTask invoked at {DateTime.Now}");
            var createTaskResponse = activityBusinessLogic.CreateTask(createTaskRequest);
            return CreatedAtAction(nameof(CreateTask), new { id = createTaskResponse.TaskId }, createTaskResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating a task.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = ex.Message,
            });
        }
    }
}