using Microsoft.Xrm.Sdk;

namespace DataverseService.UtilityArea;

public interface IDataverseNotificationService
{
    Task NotifyOwner(EntityReference recordRef, string title, string message);
}