using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace DataverseService.UtilityArea;

public class DataverseNotificationService : IDataverseNotificationService
{
    private readonly IOrganizationServiceAsync2 client;

    public DataverseNotificationService(IOrganizationServiceAsync2 client)
    {
        this.client = client;
    }

    public Task NotifyOwner(EntityReference recordRef, string title, string message)
    {
        ArgumentNullException.ThrowIfNull(recordRef);

        var record = client.Retrieve(recordRef.LogicalName, recordRef.Id, new ColumnSet("ownerid"));
        var owner = record.GetAttributeValue<EntityReference>("ownerid");

        var request = new OrganizationRequest
        {
            RequestName = "SendAppNotification",
            Parameters = new ParameterCollection
            {
                ["Title"] = title,
                ["Recipient"] = owner,
                ["Body"] = message,
                ["IconType"] = new OptionSetValue((int)appnotification_IconType.Failure),
                ["ToastType"] = new OptionSetValue((int)appnotification_ToastType.Timed),
            },
        };

        return client.ExecuteAsync(request);
    }
}