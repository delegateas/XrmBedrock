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
    /// Generic method that updates an entity of type T0 in XRM
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <param name="entity"></param>
    /// <param name="methodName"></param>
    public Task UpdateAsync<T0>(T0 entity, [CallerMemberName] string methodName = "")
        where T0 : Entity
    {
        return DataverseWrapAccessAsync.GenericUpdateInXrmAsync(logger, entity, () => CancellationTokenIsSet() ? orgServiceAsync.UpdateAsync(entity, cancellationToken) : orgServiceAsync.UpdateAsync(entity), methodName);
    }
}