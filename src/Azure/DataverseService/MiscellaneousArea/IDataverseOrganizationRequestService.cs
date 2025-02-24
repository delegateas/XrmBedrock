using Microsoft.Xrm.Sdk;

namespace DataverseService.UtilityArea;

public interface IDataverseOrganizationRequestService
{
    bool ExecuteTransactionRequest(OrganizationRequestCollection organizationRequests);

    IEnumerable<ExecuteMultipleResponseItem> Bulk<T>(IEnumerable<T> requests, bool continueOnError = false, bool throwOnErrors = true)
        where T : OrganizationRequest;

    IEnumerable<ExecuteMultipleResponseItem> Bulk(IEnumerable<OrganizationRequestCollection> requests, bool continueOnError = false, bool throwOnErrors = true);
}