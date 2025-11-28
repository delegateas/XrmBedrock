using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;

namespace ConsoleJobs.Jobs;

public class WhoAmIJob : IJob
{
    public void Run(JobContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        ctx.Logger.LogInformation("Executing WhoAmI request...");

        var response = (WhoAmIResponse)ctx.OrgService.Execute(new WhoAmIRequest());

        ctx.Logger.LogInformation("User ID: {UserId}", response.UserId);
        ctx.Logger.LogInformation("Business Unit ID: {BusinessUnitId}", response.BusinessUnitId);
        ctx.Logger.LogInformation("Organization ID: {OrganizationId}", response.OrganizationId);
    }
}
