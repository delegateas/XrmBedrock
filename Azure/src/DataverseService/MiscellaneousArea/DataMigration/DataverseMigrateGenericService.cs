using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedContext.Dao;
using SharedContext.Dao.Exceptions;
using SharedDomain;
using System.Linq.Expressions;

namespace DataverseService.UtilityArea.DataMigration;

public class DataverseMigrateGenericService : IDataverseMigrateGenericService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseMigrateGenericService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        adminDao.SetCancellationToken(cancellationToken);
    }

    public Task<OrganizationResponse> ExecuteUnawaitedAsync(OrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return adminDao.ExecuteUnawaitedAsync(request);
    }

    public T? ResolveEntity<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue, params Expression<Func<T, object?>>[] columnExpressions)
        where T : Entity
    {
        var entityLogicalName = DataverseStaticExtensions.LogicalName<T>();
        var columnSet = columnExpressions.ToList().ConvertAll(x => x.StringOf()).ToArray();
        return PerformQueryForEntityAsync(entityLogicalName, attributeMatch.StringOf(), attributeValue, columnSet).GetAwaiter().GetResult()?.ToEntity<T>();
    }

    public EntityReference? ResolveLookup<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue)
        where T : Entity
    {
        ArgumentNullException.ThrowIfNull(attributeMatch);
        var entityLogicalName = DataverseStaticExtensions.LogicalName<T>();
        return PerformQueryForEntityReferenceAsync(entityLogicalName, attributeMatch.StringOf(), attributeValue, $"{entityLogicalName}id").GetAwaiter().GetResult();
    }

    public EntityReference? ResolveLookup<T>(string attributeMatchName, object? attributeValue)
        where T : Entity
    {
        var entityLogicalName = DataverseStaticExtensions.LogicalName<T>();
        return PerformQueryForEntityReferenceAsync(entityLogicalName, attributeMatchName, attributeValue, $"{entityLogicalName}id").GetAwaiter().GetResult();
    }

    public EntityReference? ResolveLookupByRelated<T>(Expression<Func<T, object?>> attributeMatch, object? attributeValue, Expression<Func<T, EntityReference>> relatedSelect)
        where T : Entity
    {
        ArgumentNullException.ThrowIfNull(attributeMatch);
        var entityLogicalName = DataverseStaticExtensions.LogicalName<T>();
        return PerformQueryForEntityReferenceAsync(entityLogicalName, attributeMatch.StringOf(), attributeValue, relatedSelect.StringOf()).GetAwaiter().GetResult();
    }

    private async Task<Entity?> PerformQueryForEntityAsync(string entityName, string attributeMatchName, object? attributeValue, string[] columnSet)
    {
        if (attributeValue == null || (attributeValue is string && string.IsNullOrWhiteSpace((string)attributeValue)))
            return null;
        var attributeValueAsString = GetAttributeValueAsString(attributeValue);
        var columnsetAsString = string.Join(",", columnSet);
        var cacheKey = $"{entityName}_{attributeMatchName}_{attributeValueAsString}_{columnsetAsString}";
        var attributeCondition = new ConditionExpression(attributeMatchName, ConditionOperator.Equal, attributeValue);
        var stateCondition = new ConditionExpression("statecode", ConditionOperator.Equal, 0);
        var filter = new FilterExpression();
        filter.Conditions.Add(attributeCondition);
        filter.Conditions.Add(stateCondition);
        var query = new QueryExpression(entityName)
        {
            ColumnSet = new ColumnSet(columnSet),
            Criteria = filter,
        };

        try
        {
            var res = await adminDao.RetrieveListCachableAsync<Entity>(cacheKey, query);
            switch (res.Count)
            {
                case 0:
                    throw new InvalidDataException($"No records found for {entityName} with {attributeMatchName} = {attributeValue}");
                case 1:
                    return res[0];
                default:
                    throw new InvalidDataException($"Multiple records found for {entityName} with {attributeMatchName} = {attributeValue}");
            }
        }
        catch (DataverseInteractionException ex)
        {
            throw new DataverseInteractionException($"Error while trying to retrieve {entityName} with {attributeMatchName} = {attributeValueAsString} and columns {columnsetAsString}", ex);
        }
    }

    private async Task<EntityReference?> PerformQueryForEntityReferenceAsync(string entityName, string attributeMatchName, object? attributeValue, string attributeSelectName)
    {
        if (attributeValue == null || (attributeValue is string && string.IsNullOrWhiteSpace((string)attributeValue)))
            return null;
        var attributeValueAsString = GetAttributeValueAsString(attributeValue);
        var cacheKey = $"{entityName}_{attributeMatchName}_{attributeValueAsString}_{attributeSelectName}";
        var columnSet = new[] { attributeSelectName };
        var attributeCondition = new ConditionExpression(attributeMatchName, ConditionOperator.Equal, attributeValue);
        var stateCondition = new ConditionExpression("statecode", ConditionOperator.Equal, 0);
        var filter = new FilterExpression();
        filter.Conditions.Add(attributeCondition);
        filter.Conditions.Add(stateCondition);
        var query = new QueryExpression(entityName)
        {
            ColumnSet = new ColumnSet(columnSet),
            Criteria = filter,
        };

        try
        {
            var res = await adminDao.RetrieveListCachableAsync<Entity>(cacheKey, query);
            switch (res.Count)
            {
                case 0:
                    throw new InvalidDataException($"No records found for {entityName} with {attributeMatchName} = {attributeValue}");
                case 1:
                    var selectedAttribute = res[0].Attributes[attributeSelectName];
                    if (selectedAttribute == null)
                        throw new InvalidDataException($"No attribute found for {entityName} with {attributeMatchName} = {attributeValue}");
                    switch (selectedAttribute)
                    {
                        case EntityReference entityReference:
                            return entityReference;
                        case Guid guid:
                            return new EntityReference(entityName, guid);
                        default:
                            throw new InvalidDataException($"Unexpected attribute type found for {entityName} with {attributeMatchName} = {attributeValue}");
                    }

                default:
                    throw new InvalidDataException($"Multiple records found for {entityName} with {attributeMatchName} = {attributeValue}");
            }
        }
        catch (DataverseInteractionException ex)
        {
            throw new DataverseInteractionException($"Error while trying to retrieve reference to {entityName} with {attributeMatchName} = {attributeValueAsString} and column {attributeSelectName}", ex);
        }
    }

    private static string GetAttributeValueAsString(object attributeValue)
    {
        return attributeValue switch
        {
            int intValue => intValue.ToStringSolutionDefault(),
            Guid guidValue => guidValue.ToString(),
            string stringValue => stringValue,
            _ => throw new NotSupportedException($"Attribute value type {attributeValue.GetType()} is not supported"),
        };
    }
}