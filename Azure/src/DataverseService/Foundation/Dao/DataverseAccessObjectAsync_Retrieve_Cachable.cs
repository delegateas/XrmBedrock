using XrmBedrock.SharedContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.CompilerServices;
using Task = System.Threading.Tasks.Task;

namespace LF.Services.Dataverse.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Same as RetrieveAsync<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T> RetrieveCachableAsync<T>(string cacheKey, Guid guidOfElement, Func<T, T> selector, [CallerMemberName] string callerMethodName = "")
        where T : Entity => Task.Run(() => RetrieveCachable<T>(cacheKey, guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Same as RetrieveAsync<typeparamref name="T0"/>,<typeparamref name="T1"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T1> RetrieveCachableAsync<T0, T1>(string cacheKey, Guid guidOfElement, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class => Task.Run(() => RetrieveCachable<T0, T1>(cacheKey, guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Same as RetrieveAsync<typeparamref name="T0"/>,<typeparamref name="T1"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T1?> RetrieveCachableAsync<T0, T1>(string cacheKey, Guid guidOfElement, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct => Task.Run(() => RetrieveCachable<T0, T1>(cacheKey, guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Same as RetrieveListAsync<typeparamref name="T"/> but allows the use of caching of the retrieved list of objects by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="del"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveListCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> del, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveListCachable<T>(cacheKey, del, callerMethodName));

    /// <summary>
    /// Same as RetrieveListAsync<typeparamref name="T"/> but allows the use of caching of the retrieved list of objects by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="query"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveListCachableAsync<T>(string cacheKey, QueryExpression query, [CallerMemberName] string callerMethodName = "")
        where T : Entity => Task.Run(() => RetrieveListCachable<T>(cacheKey, query, callerMethodName));

    /// <summary>
    /// Same as RetrieveSingleAsync<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T> RetrieveSingleCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveSingleCachable<T>(cacheKey, delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Same as RetrieveSingleOrDefaultAsync<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T?> RetrieveSingleOrDefaultCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveSingleOrDefaultCachable<T>(cacheKey, delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Same as RetrieveFirstAsync<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T> RetrieveFirstCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveFirstCachable<T>(cacheKey, delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Same as RetrieveFirstOrDefaultAsync<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T?> RetrieveFirstOrDefaultCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveFirstOrDefaultCachable<T>(cacheKey, delegateQueryFunction, callerMethodName));
}