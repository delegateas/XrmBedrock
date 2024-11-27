using Microsoft.Xrm.Sdk;
using Task = System.Threading.Tasks.Task;

namespace LF.Services.Dataverse.Dao;

#pragma warning disable MA0048 // File name must match type name
public partial class DataverseAccessObjectAsync
#pragma warning restore MA0048 // File name must match type name
{
    /// <summary>
    /// Performs asynchronously a bulk operation, depending on request objects
    /// </summary>
    /// <typeparam name="T">Request type</typeparam>
    /// <param name="requests"></param>
    /// <param name="continueOnError">Continue to execute request if error occour</param>
    /// <param name="throwOnErrors">Throw Exception if there are errors</param>
    /// <returns></returns>
    public Task<List<ExecuteMultipleResponseItem>> BulkAsync<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true)
        where T : OrganizationRequest => Task.Run(() => Bulk<T>(requests, continueOnError, throwOnErrors));
}