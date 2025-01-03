using DataverseLogic.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Dataverse.Plugins.Utility;

/// <summary>
/// This plugin is responsible for autopublishing duplicate detection rules on PublishAll.
/// It will only publish unpublished rules that have a description containing "AutoPublish".
/// It solves the problem of duplicate detection rules being unpublished after a solution import.
/// </summary>
public class AutopublishDuplicateRulesOnPublishAll : Plugin
{
    public AutopublishDuplicateRulesOnPublishAll()
        : base(typeof(AutopublishDuplicateRulesOnPublishAll))
    {
        RegisterPluginStep<AnyEntity>(
            EventOperation.PublishAll,
            ExecutionStage.PostOperation,
            provider => provider.GetRequiredService<IDuplicateRuleService>().AutopublishRules())
            .SetExecutionMode(ExecutionMode.Asynchronous);
    }
}