using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using System.Runtime.CompilerServices;

namespace Azure.DataverseService.Foundation.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Generic Async method that creates an entity of type T0 in XRM
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <param name="entity"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Task<Guid> CreateAsync<T0>(T0 entity, [CallerMemberName] string methodName = "")
        where T0 : Entity
    {
        return DataverseWrapAccessAsync.GenericCreateInXrmAsync(logger, entity, () => CancellationTokenIsSet() ? orgServiceAsync.CreateAsync(entity, cancellationToken) : orgServiceAsync.CreateAsync(entity), methodName);
    }
}