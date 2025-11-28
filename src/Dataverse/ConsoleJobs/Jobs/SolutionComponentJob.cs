using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ConsoleJobs.Jobs;

public class SolutionComponentJob : IJob
{
    public void Run(JobContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        ctx.Logger.LogInformation("Fetching solution components modified in last 7 days...");

        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var query = new QueryExpression("solutioncomponent")
        {
            ColumnSet = new ColumnSet("objectid", "componenttype", "modifiedonbehalfby", "createdonbehalfby", "modifiedon"),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, sevenDaysAgo),
                },
            },
        };

        var results = ctx.OrgService.RetrieveMultiple(query);

        ctx.Logger.LogInformation("Found {Count} solution components", results.Entities.Count);

        var grouped = results.Entities
            .GroupBy(e => e.GetAttributeValue<EntityReference>("modifiedonbehalfby")?.Id ?? Guid.Empty)
            .ToList();

        foreach (var group in grouped)
        {
            var firstItem = group.First();
            var modifiedBy = firstItem.GetAttributeValue<EntityReference>("modifiedonbehalfby");
            var createdBy = firstItem.GetAttributeValue<EntityReference>("createdonbehalfby");

            var modifiedByName = modifiedBy?.Name ?? "(none)";
            var createdByName = createdBy?.Name ?? "(none)";

            ctx.Logger.LogInformation(
                "Modified by: {ModifiedBy} | Created by: {CreatedBy} | Count: {Count}",
                modifiedByName,
                createdByName,
                group.Count());

            foreach (var component in group)
            {
                var objectId = component.GetAttributeValue<Guid>("objectid");
                var componentType = component.GetAttributeValue<OptionSetValue>("componenttype")?.Value;
                var modifiedOn = component.GetAttributeValue<DateTime>("modifiedon");

                ctx.Logger.LogInformation(
                    "  ObjectId: {ObjectId} | ComponentType: {ComponentType} | ModifiedOn: {ModifiedOn}",
                    objectId,
                    componentType,
                    modifiedOn);
            }
        }
    }
}
