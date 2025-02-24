using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using SharedContext.Dao;
using XrmBedrock.SharedContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    /// <summary>
    /// Generic method that executes an OrganisationRequest in XRM
    /// </summary>
    /// <param name="request"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public OrganizationResponse Execute(OrganizationRequest request, [CallerMemberName] string callerMethodName = "")
    {
        return DataverseWrapAccess.WrapFunction(logger, "Execute", request.RequestName, () => orgService.Execute(request), callerMethodName);
    }
}