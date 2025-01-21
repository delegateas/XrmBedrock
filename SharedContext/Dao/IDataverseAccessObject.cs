using XrmBedrock.SharedContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;

namespace SharedContext.Dao;

public interface IDataverseAccessObject
{
    OrganizationResponse AssociateEntities(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "");
    OrganizationResponse AssociateEntities(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string callerMethodName = "");
    OrganizationResponse AssociateEntities<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "") where T : Entity;
    OrganizationResponse AssociateEntities<T>(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string callerMethodName = "") where T : Entity;
    List<ExecuteMultipleResponseItem> Bulk<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true) where T : OrganizationRequest;
    T Constructor<T>(T? entity, Action<T> modifier) where T : Entity, new();
    Guid Create<T>(T entity, [CallerMemberName] string callerMethodName = "") where T : Entity;
    void Delete<T>(Guid entityGuid, [CallerMemberName] string callerMethodName = "") where T : Entity;
    OrganizationResponse DisassociateEntities(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "");
    OrganizationResponse DisassociateEntities<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "") where T : Entity;
    OrganizationResponse Execute(OrganizationRequest request, [CallerMemberName] string callerMethodName = "");
    OrganizationResponse ExecuteCachable(string cacheKey, OrganizationRequest request, [CallerMemberName] string callerMethodName = "");
    string GetOptionSetText<T>(T entity, Expression<Func<T, object>> attributeExpression, [CallerMemberName] string callerMethodName = "") where T : Entity;
    T Producer<T>(T? entity) where T : Entity, new();
    T Producer<T>(T? entity, Action<T> modifier) where T : Entity, new();
    string ResolveNameIfEmpty<T>(EntityReference reference, Func<T, string> selector, [CallerMemberName] string callerMethodName = "") where T : Entity;
    T Retrieve<T>(Guid guid, Func<T, T> selector, [CallerMemberName] string callerMethodName = "") where T : Entity;
    T Retrieve<T>(Guid guid, params Expression<Func<T, object>>[] columnExpressions) where T : Entity;
    T1? Retrieve<T0, T1>(Guid guid, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct;
    T1 Retrieve<T0, T1>(Guid guid, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class;
    T RetrieveCachable<T>(string cacheKey, Guid guid, Func<T, T> selector, [CallerMemberName] string callerMethodName = "") where T : Entity;
    T1? RetrieveCachable<T0, T1>(string cacheKey, Guid guid, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct;
    T1 RetrieveCachable<T0, T1>(string cacheKey, Guid guid, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class;
    List<T> RetrieveList<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    List<T> RetrieveList<T>(QueryExpression query, [CallerMemberName] string callerMethodName = "") where T : Entity;
    IEnumerable<T> RetrieveIEnumerable<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T RetrieveSingle<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T? RetrieveSingleOrDefault<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T RetrieveFirst<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T? RetrieveFirstOrDefault<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T RetrieveSingleCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T? RetrieveSingleOrDefaultCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T RetrieveFirstCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    T? RetrieveFirstOrDefaultCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");

    List<T> RetrieveListCachable<T>(string cacheKey, Func<Xrm, IEnumerable<T>> del, [CallerMemberName] string callerMethodName = "");
    List<T> RetrieveListCachable<T>(string cacheKey, QueryExpression query, [CallerMemberName] string callerMethodName = "") where T : Entity;
    List<T> RetrieveMultipleByIds<T>(IEnumerable<Guid> guids, params Expression<Func<T, object>>[] columnExpressions) where T : Entity;
    List<T> RetrieveMultipleByIds<T>(IEnumerable<Guid> guids, string fieldName, params Expression<Func<T, object>>[] columnExpressions) where T : Entity;
    void Update<T>(T entity, [CallerMemberName] string callerMethodName = "") where T : Entity;
}