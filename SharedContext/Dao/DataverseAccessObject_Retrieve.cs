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
using System.Linq;

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    /// <summary>
    /// Generically retrieves the object with type <typeparamref name="T"/> with the specified guid from XRM
    /// The return type is <typeparamref name="T"/> so use the selector "x => x" if you truly want the entire object with all attributes, but
    /// if you only need a couple of values use a selector like "x => new <typeparamref name="T"/>(x.Id) { somecol = x.somecol, anothercol = x.anothercol, ... }"
    /// Please note the overload of this method where you don't even have to specify new T(...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T Retrieve<T>(Guid guid, Func<T, T> selector, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        return DataverseWrapAccess.WrapFunction(logger, $"Retrieve({guid})", typeof(T).ToString(), () =>
        {
            using (Xrm xrm = new Xrm(orgService))
            {
                try
                {
                    return xrm.CreateQuery<T>().Where(x => x.Id == guid).Select(selector).First();
                }
                catch (InvalidOperationException ioe)
                {
                    throw new Exception($"There is no {typeof(T)} with id {guid} in Xrm", ioe);
                }
            }
        }, callerMethodName);
    }

    /// <summary>
    /// Generically retrieves the object with type <typeparamref name="T"/> with the specified guid from XRM
    /// If you truly want the entire object with all attributes use the override with x => x as selector
    /// If you only need a couple of values specify these like "x => x.somecol, x => x.anothercol, ... "
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T Retrieve<T>(Guid guid, params Expression<Func<T, object>>[] columnExpressions) where T : Entity
    {
        if (columnExpressions == null || columnExpressions.Length == 0)
        {
            throw new ArgumentException("There is no specification of attributes to retrieve", nameof(columnExpressions));
        }

        Func<T, T> selector = source =>
        {
            var ret = Activator.CreateInstance<T>();
            ret.Id = source.Id;
            columnExpressions.ToList().ConvertAll(x => x.StringOf()).ToList().ForEach(c => { if (source.Attributes.Contains(c)) ret.Attributes[c] = source.Attributes[c]; });
            return ret;
        };
        return Retrieve<T>(guid, selector, new StackTrace().GetFrame(1).GetMethod().Name);
    }

    /// <summary>
    /// Generically retrieves the object with type <typeparamref name="T0"/> with the specified guid from XRM and returns either a single value (class ie string, EntityReference, ...) or
    /// another "container object" of type <typeparamref name="T1"/>
    /// If you just need a single value like Fullname of a Contact use a selector like "x => x.Fullname"
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T1 Retrieve<T0, T1>(Guid guid, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "") where T0 : Entity where T1 : class
    {
        return DataverseWrapAccess.WrapFunction(logger, $"Retrieve({guid})", typeof(T0).ToString(), () =>
        {
            using (Xrm xrm = new Xrm(orgService))
            {
                try
                {
                    return xrm.CreateQuery<T0>().Where(x => x.Id == guid).Select(selector).First();
                }
                catch (InvalidOperationException ioe)
                {
                    throw new Exception($"There is no {typeof(T0)} with id {guid} in Xrm", ioe);
                }
            }
        }, callerMethodName);
    }

    /// <summary>
    /// Generically retrieves the object with type T0 with the specified guid from XRM and returns a single value (struct ie int, DateTime, ...) of type T1
    /// Use it with a selector like "x => x.CreatedOn"
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="guid"></param>
    /// <param name="selector"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Nullable<T1> Retrieve<T0, T1>(Guid guid, Func<T0, Nullable<T1>> selector, [CallerMemberName] string callerMethodName = "") where T0 : Entity where T1 : struct
    {
        return DataverseWrapAccess.WrapFunction(logger, $"Retrieve({guid})", typeof(T0).ToString(), () =>
        {
            using (Xrm xrm = new Xrm(orgService))
            {
                try
                {
                    return xrm.CreateQuery<T0>().Where(x => x.Id == guid).Select(selector).First();
                }
                catch (InvalidOperationException ioe)
                {
                    throw new Exception($"There is no {typeof(T0)} with id {guid} in Xrm", ioe);
                }
            }
        }, callerMethodName);
    }

    /// <summary>
    /// Generically retrieves a list of objects by their Guids
    /// When field name is not specified, the default is entityname+"id", see overload of method for an alternative field name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guids"></param>
    /// <param name="columnExpressions"></param>
    /// <returns></returns>
    public List<T> RetrieveMultipleByIds<T>(IEnumerable<Guid> guids, params Expression<Func<T, object>>[] columnExpressions) where T : Entity
    {
        return RetrieveMultipleByIdsInnerMethod(new StackTrace().GetFrame(1).GetMethod().Name, guids, null, columnExpressions);
    }

    /// <summary>
    /// Generically retrieves a list of objects by their Guids
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guids"></param>
    /// <param name="fieldName">Name of the Guid-field</param>
    /// <param name="columnExpressions"></param>
    /// <returns></returns>
    public List<T> RetrieveMultipleByIds<T>(IEnumerable<Guid> guids, string fieldName, params Expression<Func<T, object>>[] columnExpressions) where T : Entity
    {
        return RetrieveMultipleByIdsInnerMethod(new StackTrace().GetFrame(1).GetMethod().Name, guids, fieldName, columnExpressions);
    }

    private List<T> RetrieveMultipleByIdsInnerMethod<T>(string callerMethodName, IEnumerable<Guid> guids, string fieldName = null, params Expression<Func<T, object>>[] columnExpressions) where T : Entity
    {
        var guidArray = guids.ToArray();
        if (guidArray.Length == 0)
        {
            return new List<T>();
        }

        var entityLogicalName = DataverseStaticExtensions.LogicalName<T>();
        var columnSet = columnExpressions.ToList().ConvertAll(x => x.StringOf()).ToArray();
        var query = new QueryExpression(entityLogicalName)
        {
            ColumnSet = columnSet.Length == 0 ? new ColumnSet(true) : new ColumnSet(columnSet),
            Criteria = new FilterExpression()
        };
        var condition = new ConditionExpression(fieldName ?? $"{entityLogicalName}id", ConditionOperator.In, guidArray);
        var filter = new FilterExpression();
        filter.Conditions.Add(condition);
        query.Criteria.AddFilter(filter);
        return RetrieveList<T>(query, callerMethodName);
    }

    /// <summary>
    /// Generically retrieves a list of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public List<T> RetrieveList<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return RetrieveEnumerable<T>(delegateQueryFunction, "RetrieveList", callerMethodName).ToList();
    }

    /// <summary>
    /// Generically retrieves a list of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public List<T> RetrieveList<T>(QueryExpression query, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        return DataverseWrapAccess.WrapFunction(logger, $"RetrieveList", typeof(T).ToString(), () =>
        {
            var entities = RetrieveMultiple(query);
            return entities.Select(e => e.ToEntity<T>()).ToList();
        }, callerMethodName);
    }

    /// <summary>
    /// Generically retrieves IEnumerable of objects <typeparamref name="T"/> from XRM.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public IEnumerable<T> RetrieveIEnumerable<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return DataverseWrapAccess.WrapFunction(logger, nameof(RetrieveIEnumerable), typeof(T).ToString(), () =>
        {
            using (Xrm xrm = new Xrm(orgService))
            {
                return delegateQueryFunction(xrm);
            }
        }, callerMethodName);
    }

    public IEnumerable<Entity> RetrieveMultiple(QueryExpression query)
    {
        var resp = orgService.RetrieveMultiple(query);

        foreach (var entity in resp.Entities)
        {
            yield return entity;
        }

        while (resp.MoreRecords)
        {
            query.PageInfo.PageNumber++;
            query.PageInfo.PagingCookie = resp.PagingCookie;
            resp = orgService.RetrieveMultiple(query);

            foreach (var entity in resp.Entities)
            {
                yield return entity;
            }
        }
    }

    /// <summary>
    /// Generically retrieves an object <typeparamref name="T"/> from XRM using Single. Will throw an exception if there are no results or more than one result.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T RetrieveSingle<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return RetrieveEnumerable<T>(delegateQueryFunction, "RetriveSingle", callerMethodName).Single();
    }

    /// <summary>
    /// Generically retrieves an object <typeparamref name="T"/> from XRM using SingleOrDefault. Will throw an exception if there are more than one result.
    /// Note that T does NOT have to be an entity. It can be any class constructed from data based on an XRM query 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T? RetrieveSingleOrDefault<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return RetrieveEnumerable<T>(delegateQueryFunction, "RetriveSingleOrDefault", callerMethodName).SingleOrDefault();

    }

    /// <summary>
    /// Generically retrieves an object <typeparamref name="T"/> from XRM using First. Will throw an exception if there are no results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T RetrieveFirst<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return RetrieveEnumerable<T>(delegateQueryFunction, "RetriveFirst", callerMethodName).First();
    }

    /// <summary>
    /// Generically retrieves an object <typeparamref name="T"/> from XRM using First. Will return null if there are no results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delegateQueryFunction"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public T? RetrieveFirstOrDefault<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "")
    {
        return RetrieveEnumerable<T>(delegateQueryFunction, "RetriveFirstOrDefault", callerMethodName).FirstOrDefault();
    }

    private IEnumerable<T?> RetrieveEnumerable<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, string operationName, [CallerMemberName] string callerMethodName = "")
    {
        return DataverseWrapAccess.WrapFunction(logger, operationName, typeof(T).ToString(), () =>
        {
            using (Xrm xrm = new Xrm(orgService))
            {
                return delegateQueryFunction(xrm);
            }
        }, callerMethodName);
    }
}