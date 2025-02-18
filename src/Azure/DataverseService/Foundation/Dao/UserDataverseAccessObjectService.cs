using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedContext.Dao;

namespace DataverseService.Foundation.Dao;

/// <summary>
/// The existence of this class may seem odd as Azure functions will never have a user context besides some Application User.
/// As SharedDataverseLogic is shared with plugin thay may require the existence of an UserDataverseAccessObjectService it will though be of no difference to AdminDataverseAccessObjectService in the use from Azure functions
/// </summary>
public class UserDataverseAccessObjectService : DataverseAccessObject, IUserDataverseAccessObjectService
{
    public UserDataverseAccessObjectService(IOrganizationServiceAsync2 organizationService, ILogger logger)
#pragma warning disable CA1062 // Validate arguments of public methods
        : base(organizationService, logger)
#pragma warning restore CA1062 // Validate arguments of public methods
    {
    }
}