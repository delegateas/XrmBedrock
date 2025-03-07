using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.DataverseService.Foundation.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Generic Async method that executes an OrganisationRequest in XRM
    /// </summary>
    /// <param name="request"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request, [CallerMemberName] string methodName = "")
    {
        return DataverseWrapAccessAsync.GenericExecuteInXrmAsync(logger, () => CancellationTokenIsSet() ? orgServiceAsync.ExecuteAsync(request, cancellationToken) : orgServiceAsync.ExecuteAsync(request), methodName);
    }
}