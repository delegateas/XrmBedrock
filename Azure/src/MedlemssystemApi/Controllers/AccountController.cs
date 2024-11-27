using DataverseService.Dto.Account;
using DataverseService.Foundation.Logging;
using MedlemssystemApi.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace MedlemssystemApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountBusinessLogic accountBusinessLogic;
    private readonly ILogger<AccountController> logger;

    public AccountController(IAccountBusinessLogic accountBusinessLogic, ILogger<AccountController> logger)
    {
        this.accountBusinessLogic = accountBusinessLogic;
        this.logger = logger;
    }

    /// <summary>
    /// Creates an account based on the provided request details.
    /// </summary>
    /// <param name="createAccountRequest">The request containing account creation details.</param>
    /// <returns>The created account information.</returns>
    /// <response code="201">Returns the created account information.</response>
    /// <response code="400">If the request is null or invalid.</response>
    [HttpPost("createAccount")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateAccountResponse>> CreateAccount(CreateAccountRequest createAccountRequest)
    {
        if (createAccountRequest == null)
        {
            return BadRequest();
        }

        logger.LogMessageInformation($"CreateAccount invoked at {DateTime.Now}");
        var accountResponse = await accountBusinessLogic.CreateAccount(createAccountRequest);
        return CreatedAtAction(nameof(CreateAccount), new { id = accountResponse.AccountId }, accountResponse);
    }

}