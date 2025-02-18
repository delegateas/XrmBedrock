using Microsoft.Xrm.Sdk;
using System.Linq.Expressions;

namespace DataverseService.UtilityArea.DataMigration;

public interface IDataverseMigrateGenericService
{
    Task<OrganizationResponse> ExecuteUnawaitedAsync(OrganizationRequest request);

    T? ResolveEntity<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue, params Expression<Func<T, object?>>[] columnExpressions)
        where T : Entity;

    EntityReference? ResolveLookup<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue)
        where T : Entity;

    EntityReference? ResolveLookup<T>(string attributeMatchName, object? attributeValue)
        where T : Entity;

    EntityReference? ResolveLookupByRelated<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue, Expression<Func<T, EntityReference>> relatedSelect)
        where T : Entity;

    void SetCancellationToken(CancellationToken cancellationToken);
}