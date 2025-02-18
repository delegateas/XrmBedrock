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
    /// Performs a bulk operation, depending on request objects
    /// </summary>
    /// <typeparam name="T">Request type</typeparam>
    /// <param name="requests"></param>
    /// <param name="continueOnError">Continue to execute request if error occour</param>
    /// <param name="throwOnErrors">Throw Exception if there are errors</param>
    /// <returns></returns>
    public List<ExecuteMultipleResponseItem> Bulk<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true) where T : OrganizationRequest
    {
        return DataverseWrapAccess.WrapFunction(
            logger,
            $"Bulk",
            typeof(T).FullName,
            () => { return BulkCore<T>(requests, continueOnError, throwOnErrors); },
            new StackTrace().GetFrame(1).GetMethod().Name
            );
    }

    private List<ExecuteMultipleResponseItem> BulkCore<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true) where T : OrganizationRequest
    {
        var result = orgService.PerformAsBulk(requests, continueOnError);
        if (throwOnErrors)
        {
            var errors = new List<OrganizationServiceFault>();
            result.ForEach(x => { if (x.Fault != null) errors.Add(x.Fault); });
            if (errors.Any())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"Bulk operation {nameof(T)} failed {errors.Count()} times, with :");
                errors.ForEach(e => builder.AppendLine($"[{e.Timestamp}] Errorcode: {e.ErrorCode} - {e.ErrorDetails} - {e.Message}"));
                throw new Exception(builder.ToString());
            }
        }
        return result;
    }
}