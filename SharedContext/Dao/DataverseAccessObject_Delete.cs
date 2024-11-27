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
    /// Generic method that deleted an entity of type <typeparamref name="T"/> in XRM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityGuid"></param>
    /// <param name="callerMethodName"></param>
    public void Delete<T>(Guid entityGuid, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        DataverseWrapAccess.WrapAction(logger, $"Delete({entityGuid})", typeof(T).Name, () => orgService.Delete(typeof(T).Name.ToLower(), entityGuid), callerMethodName);
    }
}