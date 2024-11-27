using SharedContext.Dao;
using XrmBedrock.SharedContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Task = System.Threading.Tasks.Task;

namespace DataverseService.Foundation.Dao;

public interface IDataverseAccessObjectAsync : IAdminDataverseAccessObjectService
{
    Task<OrganizationResponse> AssociateEntitiesAsync(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> AssociateEntitiesAsync(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> AssociateEntitiesAsync<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "") where T : Entity;
    Task<OrganizationResponse> AssociateEntitiesUnawaitedAsync(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> AssociateEntitiesUnawaitedAsync(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> AssociateEntitiesUnawaitedAsync<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "") where T : Entity;
    Task<Guid> CreateAsync<T0>(T0 entity, [CallerMemberName] string methodName = "") where T0 : Entity;
    Task<Guid> CreateUnawaitedAsync<T0>(T0 entity, [CallerMemberName] string methodName = "") where T0 : Entity;
    Task DeleteAsync<T>(Guid entityGuid, [CallerMemberName] string methodName = "") where T : Entity;
    Task DeleteUnawaitedAsync<T0>(Guid entityGuid, [CallerMemberName] string methodName = "") where T0 : Entity;
    Task<OrganizationResponse> DisassociateEntitiesAsync(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> DisassociateEntitiesAsync<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "") where T : Entity;
    Task<OrganizationResponse> DisassociateEntitiesUnawaitedAsync(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> DisassociateEntitiesUnawaitedAsync<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "") where T : Entity;
    Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request, [CallerMemberName] string methodName = "");
    Task<OrganizationResponse> ExecuteUnawaitedAsync(OrganizationRequest request, [CallerMemberName] string methodName = "");
    Task<T> RetrieveAsync<T>(Guid guidOfElement, Func<T, T> selector, [CallerMemberName] string callerMethodName = "") where T : Entity;
    Task<T> RetrieveAsync<T>(Guid guidOfElement, params Expression<Func<T, object?>>[] columnExpressions) where T : Entity;
    Task<T1?> RetrieveAsync<T0, T1>(Guid guidOfElement, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct;
    Task<T1> RetrieveAsync<T0, T1>(Guid guidOfElement, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class;
    Task<T> RetrieveCachableAsync<T>(string cacheKey, Guid guidOfElement, Func<T, T> selector, [CallerMemberName] string callerMethodName = "") where T : Entity;
    Task<T1?> RetrieveCachableAsync<T0, T1>(string cacheKey, Guid guidOfElement, Func<T0, T1?> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : struct;
    Task<T1> RetrieveCachableAsync<T0, T1>(string cacheKey, Guid guidOfElement, Func<T0, T1> selector, [CallerMemberName] string callerMethodName = "")
        where T0 : Entity
        where T1 : class;
    Task<T> RetrieveFirstAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T> RetrieveFirstCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T?> RetrieveFirstOrDefaultAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T?> RetrieveFirstOrDefaultCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<IEnumerable<T>> RetrieveIEnumerableAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
#pragma warning disable MA0016 // Prefer using collection abstraction instead of implementation
    Task<List<ExecuteMultipleResponseItem>> BulkAsync<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true) where T : OrganizationRequest;
    Task<List<T>> RetrieveListAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<List<T>> RetrieveListAsync<T>(QueryExpression query, [CallerMemberName] string callerMethodName = "") where T : Entity;
    Task<List<T>> RetrieveListCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> del, [CallerMemberName] string callerMethodName = "");
    Task<List<T>> RetrieveListCachableAsync<T>(string cacheKey, QueryExpression query, [CallerMemberName] string callerMethodName = "") where T : Entity;
    Task<List<T>> RetrieveMultipleByIdsAsync<T>(IEnumerable<Guid> guids, params Expression<Func<T, object>>[] columnExpressions) where T : Entity;
    Task<List<T>> RetrieveMultipleByIdsAsync<T>(IEnumerable<Guid> guids, string fieldName, params Expression<Func<T, object>>[] columnExpressions) where T : Entity;
#pragma warning restore MA0016 // Prefer using collection abstraction instead of implementation
    Task<IEnumerable<Entity>> RetrieveMultipleAsync(QueryExpression query);
    Task<T> RetrieveSingleAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T> RetrieveSingleCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T?> RetrieveSingleOrDefaultAsync<T>(Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    Task<T?> RetrieveSingleOrDefaultCachableAsync<T>(string cacheKey, Func<Xrm, IEnumerable<T>> delegateQueryFunction, [CallerMemberName] string callerMethodName = "");
    void SetCancellationToken(CancellationToken cancellationToken);
    Task UpdateAsync<T0>(T0 entity, [CallerMemberName] string methodName = "") where T0 : Entity;
    Task UpdateUnawaitedAsync<T0>(T0 entity, [CallerMemberName] string methodName = "") where T0 : Entity;
}