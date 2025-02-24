using SharedContext.Dao.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics;
using System.ServiceModel;

namespace SharedContext.Dao;

internal class DataverseWrapAccess
{
    public static T WrapFunction<T>(ILogger logger, string operationName, string targetName, Func<T> coreFunction, string callerMethodName, string additionalInfo = null)
    {
        logger.LogTrace($"{callerMethodName} - {operationName} of '{targetName}' in Xrm{additionalInfo}");
        var errorMsg = $"{callerMethodName} failed to {operationName} '{targetName}'{additionalInfo}";
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            return coreFunction();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
        finally
        {
            stopWatch.Stop();
            logger.LogTrace($"{callerMethodName} - {operationName} of {targetName} in Xrm completed. Duration: {stopWatch.Elapsed}");
        }
    }

    public static void WrapAction(ILogger logger, string operationName, string targetName, Action coreAction, string callerMethodName, string additionalInfo = null)
    {
        logger.LogTrace($"{callerMethodName} - {operationName} of '{targetName}' in Xrm{additionalInfo}");
        var errorMsg = $"{callerMethodName} failed to {operationName} '{targetName}'{additionalInfo}";
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            coreAction();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
        finally
        {
            stopWatch.Stop();
            logger.LogTrace($"{callerMethodName} - {operationName} of {targetName} in Xrm completed. Duration: {stopWatch.Elapsed}");
        }
    }

    public static string GetAttributesFormatted(Entity entity)
    {
        List<string> attributes = new List<string>();
        foreach (KeyValuePair<string, object> attribute in entity.Attributes)
            // Until we know where logs are put and which restrictions apply to logged data we will NOT log attribute values
            //    attributes.Add(attribute.Key + ": " + attribute.Value);
            attributes.Add(attribute.Key);
        var attributesFormatted = string.Join("; ", attributes);
        return attributesFormatted;
    }

    public static string GetEntityNameOfQuery(QueryBase query) =>
        query switch
        {
            QueryExpression expression => expression.EntityName,
            FetchExpression => "FetchExpression",
            _ => "",
        };

    #region Private methods
    /// <summary>
    /// Formats the errormessage for the XRM 2011 Organization service exceptions (FaultException&lt;OrganizationServiceFault&gt;)
    /// </summary>
    internal static DataverseInteractionException NewExceptionByOrganizationServiceFault(string message, FaultException<OrganizationServiceFault> e)
    {
        return new DataverseInteractionException($"{message} e.Detail.Innerfault = {e.Detail?.InnerFault?.Message}, e.Detail.ErrorCode = 0x{e.Detail?.ErrorCode}, e.Detail.Message = {e.Detail?.Message}, e.Detail.Innerfault.Innerfault = {e.Detail?.InnerFault?.InnerFault?.Message}", e);
    }
    /// <summary>
    /// Formats the errormessage for a generic WCF FaultException
    /// </summary>
    internal static DataverseInteractionException NewExceptionByFaultException(string message, FaultException e)
    {
        return new DataverseInteractionException($"{message} Unknown WCF exception thrown. - e.Message = {e.Message}, stacktrace = {e.StackTrace}", e);
    }

    internal static DataverseInteractionException NewExceptionByDefaultException(string errorMsg, Exception e)
    {
        return new DataverseInteractionException(errorMsg + " Details: " + e.Message, e);
    }
    #endregion Private methods
}
