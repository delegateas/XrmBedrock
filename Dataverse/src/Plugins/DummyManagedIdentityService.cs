using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Medlemssystem.Plugins;

internal sealed class DummyManagedIdentityService : IManagedIdentityService
{
    public string AcquireToken(IEnumerable<string> scopes) => "invalidtokenonlyfortest";

    public string AcquireToken(Guid managedIdentityId, IEnumerable<string> scopes) => "invalidtokenonlyfortest";
}