using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.DataverseService.Foundation.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Generic method that Disassociates two entities in XRM via the N:N relationship T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="relatedEntity"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Task<OrganizationResponse> DisassociateEntitiesAsync<T>(EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "")
        where T : Entity
    {
        var request = new DisassociateRequest
        {
            Target = target,
            RelatedEntities = new EntityReferenceCollection { relatedEntity },
            Relationship = new Relationship(typeof(T).Name),
        };
        return ExecuteAsync(request, methodName);
    }

    /// <summary>
    /// Generic method that Disassociates two entities in XRM via the N:N relationship relationshipName (this is an overload of DisassociateEntities which is only introduced due to tooling issues regarding the casing of relationshipnames)
    /// </summary>
    /// <param name="relationshipName"></param>
    /// <param name="target"></param>
    /// <param name="relatedEntity"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Task<OrganizationResponse> DisassociateEntitiesAsync(string relationshipName, EntityReference target, EntityReference relatedEntity, [CallerMemberName] string methodName = "")
    {
        var request = new DisassociateRequest
        {
            Target = target,
            RelatedEntities = new EntityReferenceCollection { relatedEntity },
            Relationship = new Relationship(relationshipName),
        };
        return ExecuteAsync(request, methodName);
    }
}