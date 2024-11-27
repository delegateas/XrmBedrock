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
    /// Generic method that deleted an entity of type T0 in XRM
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <param name="entityGuid"></param>
    /// <param name="methodName"></param>
    public Task DeleteUnawaitedAsync<T0>(Guid entityGuid, [CallerMemberName] string methodName = "")
        where T0 : Entity
    {
        var entityName = typeof(T0).Name.ToLowerInvariant();
        var deleteOperation = () => CancellationTokenIsSet() ? orgServiceAsync.DeleteAsync(entityName, entityGuid, cancellationToken) : orgServiceAsync.DeleteAsync(entityName, entityGuid);
        return DataverseWrapAccessAsync.GenericDeleteInXrmUnawaitedAsync(logger, entityGuid, typeof(T0).Name, deleteOperation, methodName);
    }
}