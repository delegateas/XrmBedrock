using Dataverse.PluginLogic.ExampleCustomerArea;
using Microsoft.Extensions.DependencyInjection;
using XrmBedrock.SharedContext;

namespace Dataverse.Plugins.CustomerArea;

/// <summary>
/// This plugin is purely for demonstration purposes and should be removed from the solution, when you have created a few real plugins in project.
/// This plugin gives a simple illustration of validating the data on an object (here an Account) before it is saved to the database.
///
/// A good practice is to describe what a plugin does in natural language. For this plugin it would be something like:
/// This plugin validates the Telephone1 field on the Account entity to ensure it starts with "+" and contains only spaces and digits.
/// </summary>
public class ExampleValidatePhoneNumber : Plugin
{
    public ExampleValidatePhoneNumber()
    {
        RegisterPluginStep<Account>(
            EventOperation.Create,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ExampleCustomerService>().ValidatePhoneNumber());

        RegisterPluginStep<Account>(
            EventOperation.Update,
            ExecutionStage.PreValidation,
            provider => provider.GetRequiredService<ExampleCustomerService>().ValidatePhoneNumber())
            .AddFilteredAttributes(
                a => a.Telephone1)
            .AddImage(
                ImageType.PreImage,
                a => a.Telephone1);
    }
}