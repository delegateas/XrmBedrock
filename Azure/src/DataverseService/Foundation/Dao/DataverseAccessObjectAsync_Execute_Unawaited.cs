using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LF.Services.Dataverse.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Generic UnawaitedAsync method that executes an OrganisationRequest in XRM
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <param name="request"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Task<OrganizationResponse> ExecuteUnawaitedAsync(OrganizationRequest request, [CallerMemberName] string methodName = "")
    {
        var executeMethod = () => CancellationTokenIsSet() ? orgServiceAsync.ExecuteAsync(request, cancellationToken) : orgServiceAsync.ExecuteAsync(request);
        return DataverseWrapAccessAsync.GenericExecuteInXrmUnawaitedAsync(logger, executeMethod, methodName);
    }
}