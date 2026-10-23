using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedContext.Dao;

namespace DataverseService.Foundation.Dao;

/// <summary>
/// This is used by the services in the DataverseService project
/// </summary>
#pragma warning disable CA1062 // Validate arguments of public methods
public class AdminDataverseAccessObjectService(IOrganizationServiceAsync2 organizationService, ILogger<AdminDataverseAccessObjectService> logger)
    : DataverseAccessObject(organizationService, logger), IAdminDataverseAccessObjectService
#pragma warning restore CA1062 // Validate arguments of public methods
{
}
