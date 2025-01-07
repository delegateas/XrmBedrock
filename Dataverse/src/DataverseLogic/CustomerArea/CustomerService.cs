using DataverseLogic.Azure;
using Microsoft.Xrm.Sdk;
using SharedDomain.CustomerArea;
using XrmBedrock.SharedContext;

namespace DataverseLogic.CustomerArea;

public class CustomerService : ICustomerService
{
    private readonly IPluginExecutionContext context;
    private readonly IAzureService azureService;

    public CustomerService(
        IPluginExecutionContext context,
        IAzureService azureService)
    {
        this.context = context;
        this.azureService = azureService;
    }

    public void DemoAddMessageToAzureQueueOnCreate()
    {
        var target = context.GetTarget<Account>();
        if (target.PrimaryContactId != null)
            SendMessageToAzureQueue();
    }

    public void DemoAddMessageToAzureQueueOnUpdate()
    {
        var target = context.GetTarget<Account>();
        var preImage = context.GetRequiredPreImage<Account>();

        // if the primary contact is actually changed and not just "touched", we send a message to the Azure queue
        if (target.PrimaryContactId?.Id != preImage.PrimaryContactId?.Id)
            SendMessageToAzureQueue();
    }

    private void SendMessageToAzureQueue()
    {
        var message = new DemoAccountMessage(context.PrimaryEntityId);
        azureService.SendDemoAccountMessage(message);
    }
}