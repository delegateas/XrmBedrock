using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace DataverseLogic;

public class AdminDataverseAccessObjectService : DataverseAccessObject, IAdminDataverseAccessObjectService
{
    public AdminDataverseAccessObjectService(IOrganizationServiceFactory organizationServiceFactory, ILogger logger)
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
        : base(organizationServiceFactory?.CreateOrganizationService(null), logger)
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
    {
    }
}