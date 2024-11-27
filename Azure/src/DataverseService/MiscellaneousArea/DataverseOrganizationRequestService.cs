using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using SharedDomain;
using System.ServiceModel;

namespace DataverseService.UtilityArea;

/// <summary>
/// The main purpose of this service is to execute a transaction request that are not yet supported by XrmMockUp.
/// With this service, it is possible to specifically mock request that are not supported by XrmMockUp.
/// </summary>
public class DataverseOrganizationRequestService : IDataverseOrganizationRequestService
{
    private readonly IDataverseAccessObjectAsync adminDao;
    private readonly ILoggingComponent logger;

    public DataverseOrganizationRequestService(IDataverseAccessObjectAsync adminDao, ILoggingComponent logger)
    {
        this.adminDao = adminDao;
        this.logger = logger;
    }

    public bool ExecuteTransactionRequest(OrganizationRequestCollection organizationRequests)
    {
        try
        {
            var orgCount = organizationRequests != null ? organizationRequests.Count : 0;
            logger.LogInformation(message: $"Executing transaction request with '{orgCount}' request(s).");
            var transactionRequest = new ExecuteTransactionRequest { Requests = organizationRequests, ReturnResponses = true };
            adminDao.Execute(transactionRequest);
            return true;
        }
        catch (FaultException<OrganizationServiceFault> ex)
        {
            var index = ((ExecuteTransactionFault)ex.Detail).FaultedRequestIndex + 1;
            var message = ex.Detail.Message;
            logger.LogError($"Method '{nameof(ExecuteTransactionRequest)}' failed to complete ExecuteTransactionRequest. Failed at index '{index}' with message: {message}.");
            return false;
        }
    }

    public IEnumerable<ExecuteMultipleResponseItem> Bulk<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true)
        where T : OrganizationRequest
    {
        return adminDao.Bulk(requests, continueOnError, throwOnErrors);
    }

    public IEnumerable<ExecuteMultipleResponseItem> Bulk(IEnumerable<OrganizationRequestCollection> requests, bool continueOnError = false, bool throwOnErrors = true)
    {
        var executeTransactionRequests = (from request in requests
                                          let transaction = new ExecuteTransactionRequest { Requests = request, ReturnResponses = true }
                                          select transaction).ToList();

        return adminDao.Bulk(executeTransactionRequests, continueOnError, throwOnErrors);
    }
}