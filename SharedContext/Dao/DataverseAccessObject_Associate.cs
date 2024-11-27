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
    /// Generic method that Associates two entities in XRM via the N:N relationship <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="relatedEntity"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public OrganizationResponse AssociateEntities<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        return AssociateEntities(typeof(T).Name, target, relatedEntity, callerMethodName);
    }

    /// <summary>
    /// Generic method that Associates two entities in XRM via the N:N relationship relationshipName (this is an overload of AssociateEntities which is only introduced due to tooling issues regarding the casing of relationshipnames)
    /// </summary>
    /// <param name="relationshipName"></param>
    /// <param name="target"></param>
    /// <param name="relatedEntity"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public OrganizationResponse AssociateEntities(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string callerMethodName = "")
    {
        return AssociateEntities(relationshipName, target, new List<EntityReference> { relatedEntity }, callerMethodName);
    }

    /// <summary>
    /// Generic method that Associates an entity to a list of entities in XRM via the N:N relationship <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="relationshipName"></param>
    /// <param name="target"></param>
    /// <param name="relatedEntities"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public OrganizationResponse AssociateEntities<T>(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        return AssociateEntities(typeof(T).Name, target, relatedEntities, callerMethodName);
    }

    /// <summary>
    /// Generic method that Associates an entity to a list of entities in XRM via the N:N relationship relationshipName (this is an overload of AssociateEntities which is only introduced due to tooling issues regarding the casing of relationshipnames)
    /// </summary>
    /// <param name="relationshipName"></param>
    /// <param name="target"></param>
    /// <param name="relatedEntities"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public OrganizationResponse AssociateEntities(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, [CallerMemberName] string callerMethodName = "")
    {
        return DataverseWrapAccess.WrapFunction(
            logger,
            $"Associate({target.Id},[{relatedEntities.Aggregate("", (s, e) => s + e.Id + ",")}])",
            relationshipName,
            () => { return AssociateEntitiesCore(relationshipName, target, relatedEntities, callerMethodName); },
            callerMethodName
            );
    }

    private OrganizationResponse AssociateEntitiesCore(string relationshipName, EntityReference target, IEnumerable<EntityReference> relatedEntities, string callerMethodName)
    {
        var collection = new EntityReferenceCollection();
        collection.AddRange(relatedEntities);
        var request = new AssociateRequest
        {
            Target = target,
            RelatedEntities = collection,
            Relationship = new Relationship(relationshipName),
        };
        return Execute(request, callerMethodName);
    }
}