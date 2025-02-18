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
    /// Will get the name of the EntityReference unless it's null then it will be resolved by a request to Dataverse.
    /// The need for this method comes from a limitation/bug in Dataverse where the entityreferences in the target property bag doesn't have the name resolved
    /// </summary>
    /// <typeparam name="T">The type of the referenced object</typeparam>
    /// <param name="reference">The entity reference in play</param>
    /// <param name="selector">The selector of the name-column of the table</param>
    /// <returns>The name of the referred object</returns>
    public string ResolveNameIfEmpty<T>(EntityReference reference, Func<T, string> selector, [CallerMemberName] string callerMethodName = "")
        where T : Entity
    {
        if (reference == null)
        {
            return string.Empty;
        }

        return reference.Name ?? Retrieve<T, string>(reference.Id, selector, callerMethodName);
    }
}