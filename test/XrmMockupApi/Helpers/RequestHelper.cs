using Microsoft.Xrm.Sdk;
using System.Reflection;

namespace XrmMockupApi.Helpers;

public static class RequestHelper
{
    public static OrganizationRequest CreateOrganizationRequest(string requestName, Dictionary<string, object> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var requestType = Type.GetType($"Microsoft.Xrm.Sdk.Messages.{requestName}");
        if (requestType == null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                requestType = assembly.GetType($"Microsoft.Xrm.Sdk.Messages.{requestName}");
                if (requestType != null) break;
            }
        }

        var genericRequest = new OrganizationRequest(requestName);
        foreach (var param in parameters)
        {
            genericRequest.Parameters[param.Key] = param.Value;
        }

        return genericRequest;
    }
}