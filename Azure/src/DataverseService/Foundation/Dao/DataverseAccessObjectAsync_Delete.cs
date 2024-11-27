using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Generic method that deleted an entity of type T in XRM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityGuid"></param>
    /// <param name="methodName"></param>
    public Task DeleteAsync<T>(Guid entityGuid, [CallerMemberName] string methodName = "")
        where T : Entity
    {
        var entityName = typeof(T).Name.ToLowerInvariant();
        var deleteOperation = () => CancellationTokenIsSet() ? orgServiceAsync.DeleteAsync(entityName, entityGuid, cancellationToken) : orgServiceAsync.DeleteAsync(entityName, entityGuid);
        return DataverseWrapAccessAsync.GenericDeleteInXrmAsync(logger, entityGuid, typeof(T).Name, deleteOperation, methodName);
    }
}