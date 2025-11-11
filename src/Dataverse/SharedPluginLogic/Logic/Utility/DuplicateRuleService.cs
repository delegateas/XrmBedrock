using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using SharedContext.Dao;
using SharedDomain;
using XrmBedrock.SharedContext;

namespace DataverseLogic.Utility;

public class DuplicateRuleService
{
    private readonly ILogger logger;
    private readonly IAdminDataverseAccessObjectService adminDao;

    public DuplicateRuleService(
        ILogger logger,
        IAdminDataverseAccessObjectService adminDao)
    {
        this.logger = logger;
        this.adminDao = adminDao;
    }

    public void AutopublishRules()
    {
        var rules = GetUnpublishedDuplicateRulesForAutoPublish();
        if (rules.Count == 0)
        {
            logger.LogInformation("No rules to autopublish.");
        }
        else
        {
            rules.ForEach(PublishRule);
        }
    }

    private List<DuplicateRule> GetUnpublishedDuplicateRulesForAutoPublish()
    {
        var unpublisedRules = adminDao.RetrieveList(xrm => xrm.DuplicateRuleSet
            .Where(r => r.StateCode == DuplicateRuleState.Inactive)
            .Select(r => new DuplicateRule
            {
                Id = r.Id,
                Description = r.Description,
                Name = r.Name,
            }));
        return unpublisedRules.Where(r => r.Description?.ToLowerSolutionDefault()?.Contains("autopublish") ?? false).ToList();
    }

    private void PublishRule(DuplicateRule rule)
    {
        logger.LogInformation("Publishing rule {Name}.", rule.Name);
        var publishRequest = new PublishDuplicateRuleRequest
        {
            DuplicateRuleId = rule.Id,
        };
        adminDao.Execute(publishRequest);
    }
}