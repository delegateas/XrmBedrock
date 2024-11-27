using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace Azure.DataverseService.Foundation.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Generically Async method that retrieves the object with type <typeparamref name="T"/> with the specified guid from XRM
    /// The return type is <typeparamref name="T"/> so use the selector "x => x" if you truly want the entire object with all attributes, but
    /// if you only need a couple of values use a selector like "x => new <typeparamref name="T"/>(x.Id) { somecol = x.somecol, anothercol = x.anothercol, ... }"
    /// Please note the overload of this method where you don't even have to specify new T(...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Thrown if element is not found</exception>
    public Task<T> RetrieveAsync<T>(Guid guidOfElement, Func<T, T> selector, [CallerMemberName] string callerMethodName = "")
        where T : Entity => Task.Run(() => Retrieve<T>(guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves the object with type <typeparamref name="T"/> with the specified guid from XRM
    /// If you truly want the entire object with all attributes use the override with x => x as selector
    /// If you only need a couple of values specify these like "x => x.somecol, x => x.anothercol, ... "
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guidOfElement"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Task<T> RetrieveAsync<T>(Guid guidOfElement, params Expression<Func<T, object?>>[] columnExpressions)
        where T : Entity => Task.Run(() => Retrieve<T>(guidOfElement, columnExpressions));

    /// <summary>
    /// Generically asynchronously retrieves the object with type <typeparamref name="T0"/> with the specified guid from XRM and returns either a single value (class ie string, EntityReference, ...) or
    /// another "container object" of type <typeparamref name="T1"/>
    /// If you just need a single value like Fullname of a Contact use a selector like "x => x.Fullname"
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T1> RetrieveAsync<T0, T1>(Guid guidOfElement, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class => Task.Run(() => Retrieve<T0, T1>(guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves the object with type T0 with the specified guid from XRM and returns a single value (struct ie int, DateTime, ...) of type T1
    /// Use it with a selector like "x => x.CreatedOn"
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="guidOfElement"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T1?> RetrieveAsync<T0, T1>(Guid guidOfElement, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct => Task.Run(() => Retrieve<T0, T1>(guidOfElement, selector, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves a list of objects by their Guids
    /// When field name is not specified, the default is entityname+"id", see overload of method for an alternative field name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guids"></param>
    /// <param name="columnExpressions"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveMultipleByIdsAsync<T>(IEnumerable<Guid> guids, params Expression<Func<T, object>>[] columnExpressions)
        where T : Entity => Task.Run(() => RetrieveMultipleByIds<T>(guids, columnExpressions));

    /// <summary>
    /// Generically asynchronously retrieves a list of objects by their Guids
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guids"></param>
    /// <param name="fieldName">Name of the Guid-field</param>
    /// <param name="columnExpressions"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveMultipleByIdsAsync<T>(IEnumerable<Guid> guids, string fieldName, params Expression<Func<T, object>>[] columnExpressions)
        where T : Entity => Task.Run(() => RetrieveMultipleByIds<T>(guids, fieldName, columnExpressions));

    /// <summary>
    /// Generically asynchronously retrieves a list of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveListAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveList<T>(delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves a list of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<List<T>> RetrieveListAsync<T>(QueryExpression query, [CallerMemberName] string callerMethodName = "")
        where T : Entity => Task.Run(() => RetrieveList<T>(query, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves IEnumerable of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<IEnumerable<T>> RetrieveIEnumerableAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveIEnumerable<T>(delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves IEnumerable of Entities from XRM.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Task<IEnumerable<Entity>> RetrieveMultipleAsync(QueryExpression query)
        => Task.Run(() => RetrieveMultiple(query));

    /// <summary>
    /// Generically asynchronously retrieves an object <typeparamref name="T"/> from XRM using Single. Will throw an exception if there are no results or more than one result.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T> RetrieveSingleAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveSingle<T>(delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves an object <typeparamref name="T"/> from XRM using SingleOrDefault. Will throw an exception if there are more than one result.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T?> RetrieveSingleOrDefaultAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveSingleOrDefault<T>(delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves an object <typeparamref name="T"/> from XRM using First. Will throw an exception if there are no results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T> RetrieveFirstAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveFirst<T>(delegateQueryFunction, callerMethodName));

    /// <summary>
    /// Generically asynchronously retrieves an object <typeparamref name="T"/> from XRM using First. Will return null if there are no results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Task<T?> RetrieveFirstOrDefaultAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
        => Task.Run(() => RetrieveFirstOrDefault<T>(delegateQueryFunction, callerMethodName));
}