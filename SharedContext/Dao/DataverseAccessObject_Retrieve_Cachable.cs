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
using System.Collections.Generic;

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    /// <summary>
    /// Same as Retrieve<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T RetrieveCachable<T>(string cacheKey, Guid guid, Func<T, T> selector, [CallerMemberName] string callerMethodName = "")
        where T : Entity
    {
        return cacheHandler.GetOrCreate<T>(cacheKey, () => Retrieve<T>(guid, selector, callerMethodName));
    }

    /// <summary>
    /// Same as Retrieve<typeparamref name="T0"/>,<typeparamref name="T1"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T1 RetrieveCachable<T0, T1>(string cacheKey, Guid guid, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class
    {
        return cacheHandler.GetOrCreate<T1>(cacheKey, () => Retrieve<T0, T1>(guid, selector, callerMethodName));
    }

    /// <summary>
    /// Same as Retrieve<typeparamref name="T0"/>,<typeparamref name="T1"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T1? RetrieveCachable<T0, T1>(string cacheKey, Guid guid, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct
    {
        return cacheHandler.GetOrCreate<T1?>(cacheKey, () => Retrieve<T0, T1>(guid, selector, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveList<typeparamref name="T"/> but allows the use of caching of the retrieved list of objects by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="del"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public List<T> RetrieveListCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> del, [CallerMemberName] string callerMethodName = "")
    {
        return cacheHandler.GetOrCreate<List<T>>(cacheKey, () => RetrieveList<T>(del, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveList<typeparamref name="T"/> but allows the use of caching of the retrieved list of objects by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="query"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public List<T> RetrieveListCachable<T>(string cacheKey, QueryExpression query, [CallerMemberName] string callerMethodName = "")
        where T : Entity
    {
        return cacheHandler.GetOrCreate<List<T>>(cacheKey, () => RetrieveList<T>(query, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveSingle<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T RetrieveSingleCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return cacheHandler.GetOrCreate<T>(cacheKey, () => RetrieveSingle(delegateQueryFunction, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveSingleOrDefault<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T? RetrieveSingleOrDefaultCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return cacheHandler.GetOrCreate<T>(cacheKey, () => RetrieveSingleOrDefault(delegateQueryFunction, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveFirst<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T RetrieveFirstCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return cacheHandler.GetOrCreate<T>(cacheKey, () => RetrieveFirst(delegateQueryFunction, callerMethodName));
    }

    /// <summary>
    /// Same as RetrieveFirstOrDefault<typeparamref name="T"/> but allows the use of caching of the retrieved object by the specified cacheKey
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T? RetrieveFirstOrDefaultCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return cacheHandler.GetOrCreate<T>(cacheKey, () => RetrieveFirstOrDefault(delegateQueryFunction, callerMethodName));
    }
}