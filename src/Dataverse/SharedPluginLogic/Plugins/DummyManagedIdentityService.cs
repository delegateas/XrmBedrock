using Microsoft.Xrm.Sdk;

namespace Dataverse.Plugins;

internal sealed class DummyManagedIdentityService : IManagedIdentityService
{
    public string AcquireToken(IEnumerable<string> scopes) => "invalidtokenonlyfortest";

    public string AcquireToken(Guid managedIdentityId, IEnumerable<string> scopes) => "invalidtokenonlyfortest";
}