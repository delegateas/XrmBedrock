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

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    /// <summary>
    /// Get text (label) for an OptionSet value
    /// Also works for Status-fields
    /// Note! Uses caching!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="attributeExpression"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public string GetOptionSetText<T>(T entity, Expression<Func<T, object>> attributeExpression, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        // Get the name of the attribute from the expression
        var attributeName = attributeExpression.StringOf<T>();
        // Get the value of the attribute from the entity using the expression, need to check for null _before_ casting!
        var optionSetValueUntyped = attributeExpression.Compile()(entity);
        if (optionSetValueUntyped == null)
            return null;
        var optionSetValue = (int)optionSetValueUntyped;
        // Get the metadata of the attributes of the entity
        var metadata = GetEntityAttributeDataCachable<T>(callerMethodName).EntityMetadata;
        // Find the metadata of the specific attribute
        var attributeMetadata = metadata.Attributes.FirstOrDefault(o => string.Equals(o.LogicalName, attributeName, StringComparison.OrdinalIgnoreCase));
        // Find the label for the value of the attribute
        return GetOptionsFromAttributeMetadata(attributeMetadata).Options.Where(o => o.Value.Value == optionSetValue).First().Label.UserLocalizedLabel.Label;
    }

    private RetrieveEntityResponse GetEntityAttributeDataCachable<T>([CallerMemberName] string callerMethodName = "") where T : Entity
    {
        string entityName = DataverseStaticExtensions.LogicalName<T>();
        return cacheHandler.GetOrCreate($"EntityAttributeData_{entityName}", () => GetEntityAttributeData(entityName, callerMethodName));
    }

    private RetrieveEntityResponse GetEntityAttributeData(string entityName, [CallerMemberName] string callerMethodName = "")
    {
        var retrieveDetails = new RetrieveEntityRequest
        {
            EntityFilters = EntityFilters.Attributes,
            LogicalName = entityName
        };
        return (RetrieveEntityResponse)Execute(retrieveDetails, callerMethodName);
    }

    private OptionSetMetadata GetOptionsFromAttributeMetadata(AttributeMetadata attributeMetadata)
    {
        switch (attributeMetadata)
        {
            case PicklistAttributeMetadata a:
                return a.OptionSet;
            case StatusAttributeMetadata a:
                return a.OptionSet;
            default:
                throw new Exception($"Invalid attempt to treat attribute {attributeMetadata.LogicalName} as an optionset-like attribute. It's of type {attributeMetadata.AttributeTypeName}");
        }
    }
}