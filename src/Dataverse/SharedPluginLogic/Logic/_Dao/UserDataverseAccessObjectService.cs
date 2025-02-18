using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace DataverseLogic;

public class UserDataverseAccessObjectService : DataverseAccessObject, IUserDataverseAccessObjectService
{
    public UserDataverseAccessObjectService(IOrganizationServiceFactory organizationServiceFactory, IPluginExecutionContext context, ILogger logger)
#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA2254 // Template should be a static expression
        : base(organizationServiceFactory?.CreateOrganizationService(context.UserId), logger)
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1062 // Validate arguments of public methods
    {
    }
}