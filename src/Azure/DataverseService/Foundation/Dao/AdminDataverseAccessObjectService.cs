using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedContext.Dao;

namespace DataverseService.Foundation.Dao;

/// <summary>
/// This is used by the services in the SharedDataverseLogic project
/// </summary>
public class AdminDataverseAccessObjectService : DataverseAccessObject, IAdminDataverseAccessObjectService
{
    public AdminDataverseAccessObjectService(IOrganizationServiceAsync2 organizationService, ILogger<AdminDataverseAccessObjectService> logger)
#pragma warning disable CA1062 // Validate arguments of public methods
        : base(organizationService, logger)
#pragma warning restore CA1062 // Validate arguments of public methods
    {
    }
}