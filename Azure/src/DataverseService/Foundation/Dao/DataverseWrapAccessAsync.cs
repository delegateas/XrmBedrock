using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao.Exceptions;
using System.Diagnostics;
using System.ServiceModel;

namespace DataverseService.Foundation.Dao;

public static class DataverseWrapAccessAsync
{
    #region generic create

    /// <summary>
    /// Generic method that wraps "plumbing" when creating an object in XRM
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entity"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static async Task<Guid> GenericCreateInXrmAsync(ILogger logger, Entity entity, Func<Task<Guid>> coremethod, string methodName)
    {
        var entityName = entity.GetType().Name;
        var attributesFormatted = GetAttributesFormatted(entity);
        logger.LogTrace($"{methodName} - Creates '{entityName}' in XRM with data '{attributesFormatted}'");
        var errorMsg = $"{methodName} failed creating '{entityName}' with data: '{attributesFormatted}'";
        try
        {
            return await coremethod();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (System.Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
    }

    /// <summary>
    /// Generic method that add a bit of logging when creating an object in XRM.
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entity"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static Task<Guid> GenericCreateInXrmUnawaitedAsync(ILogger logger, Entity entity, Func<Task<Guid>> coremethod, string methodName)
    {
        var entityName = entity.GetType().Name;
        var attributesFormatted = GetAttributesFormatted(entity);
        logger.LogTrace($"{methodName} - Creates '{entityName}' in XRM with data '{attributesFormatted}'");
        return coremethod();
    }

    #endregion generic create

    #region generic update

    /// <summary>
    /// Generic method that wraps "plumbing" when updating an object in XRM
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entity"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    public static async Task GenericUpdateInXrmAsync(ILogger logger, Entity entity, Func<Task> coremethod, string methodName)
    {
        var entityName = entity.GetType().Name;
        var attributesFormatted = GetAttributesFormatted(entity);
        logger.LogTrace($"{methodName} - Updates '{entity.Id}' of type '{entityName}' with data '{attributesFormatted}'");
        var errorMsg = $"{methodName} failed updating '{entity.Id}' of type '{entityName}' with data '{attributesFormatted}'";
        try
        {
            await coremethod();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (System.Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
    }

    /// <summary>
    /// Generic method that add a bit of logging when updating an object in XRM.
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entity"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    public static Task GenericUpdateInXrmUnawaitedAsync(ILogger logger, Entity entity, Func<Task> coremethod, string methodName)
    {
        var entityName = entity.GetType().Name;
        var attributesFormatted = GetAttributesFormatted(entity);
        logger.LogTrace($"{methodName} - Updates '{entity.Id}' of type '{entityName}' with data '{attributesFormatted}'");
        return coremethod();
    }

    #endregion generic update

    #region generic retrieve

    /// <summary>
    /// Generic method that wraps "plumbing" when retrieving data from XRM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="coremethod"></param>
    /// <returns></returns>
    public static async Task<T> GenericRetrieveFromXrmAsync<T>(ILogger logger, Func<Task<T>> coremethod, string methodName)
    {
        var errorMsg = $"{methodName} failed to retrieve {typeof(T)} in XRM";
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            return await coremethod();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (System.Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
        finally
        {
            stopWatch.Stop();
            logger?.LogInformation($"{methodName} - Retrieved {typeof(T)} in XRM. Duration: {stopWatch.Elapsed}");
        }
    }

    /// <summary>
    /// Generic method that add a bit of logging when retrieving data from XRM.
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="coremethod"></param>
    /// <returns></returns>
    public static Task<T0> GenericRetrieveFromXrmUnawaitedAsync<T0>(ILogger logger, Func<Task<T0>> coremethod, string methodName)
    {
        logger.LogTrace($"{methodName} - Retrieves {typeof(T0)} in XRM");
        return coremethod();
    }

    #endregion generic retrieve

    #region Generic execute

    /// <summary>
    /// Generic method that wraps "plumbing" when executing an organizationrequest in XRM
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static async Task<OrganizationResponse> GenericExecuteInXrmAsync(ILogger logger, Func<Task<OrganizationResponse>> coremethod, string methodName)
    {
        logger.LogTrace($"{methodName} - Executes request in XRM");
        var errorMsg = $"{methodName} failed to execute request in XRM";
        try
        {
            return await coremethod();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (System.Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
    }

    /// <summary>
    /// Generic method that add a bit of logging when executing an organizationrequest in XRM.
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="coremethod"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static Task<OrganizationResponse> GenericExecuteInXrmUnawaitedAsync(ILogger logger, Func<Task<OrganizationResponse>> coremethod, string methodName)
    {
        logger.LogTrace($"{methodName} - Executes request in XRM");
        return coremethod();
    }

    #endregion Generic execute

    #region generic delete

    /// <summary>
    /// Generic method that wraps "plumbing" when deleting an object in XRM
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entityGuid"></param>
    /// <param name="entityType"></param>
    /// <param name="coremethod"></param>
    /// <returns></returns>
    public static async Task GenericDeleteInXrmAsync(ILogger logger, Guid entityGuid, string entityType, Func<Task> coremethod, string methodName)
    {
        logger.LogTrace($"{methodName} - Deletes '{entityType}' with id '{entityGuid}' in XRM");
        var errorMsg = $"{methodName} failed to delete '{entityType}' with id '{entityGuid}' in XRM";
        try
        {
            await coremethod();
        }
        #region Standard Exceptionhandling for XRM access
        catch (InvalidPluginExecutionException) { throw; }
        catch (FaultException<OrganizationServiceFault> e) { throw NewExceptionByOrganizationServiceFault(errorMsg, e); }
        catch (FaultException e) { throw NewExceptionByFaultException(errorMsg, e); }
        catch (System.Exception e) { throw NewExceptionByDefaultException(errorMsg, e); }
        #endregion Standard Exceptionhandling for XRM access
    }

    /// <summary>
    /// Generic method that add a bit of logging when deleting an object in XRM.
    /// NOTE! This is an UnawaitedAsync method so caller is responsible for all exception handling. Please keep it uniform!
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entityGuid"></param>
    /// <param name="entityType"></param>
    /// <param name="coremethod"></param>
    /// <returns></returns>
    public static Task GenericDeleteInXrmUnawaitedAsync(ILogger logger, Guid entityGuid, string entityType, Func<Task> coremethod, string methodName)
    {
        logger.LogTrace($"{methodName} - Deletes '{entityType}' with id '{entityGuid}' in XRM");
        return coremethod();
    }

    #endregion generic delete

    #region Private methods

    private static string GetAttributesFormatted(Entity entity)
    {
        var attributes = new List<string>();
        foreach (var attribute in entity.Attributes)
            attributes.Add(attribute.Key); // Until we know where logs are put and which restrictions apply to logged data we will NOT log attribute values it would be possible here though
        var attributesFormatted = string.Join("; ", attributes);
        return attributesFormatted;
    }

    /// <summary>
    /// Formats the errormessage for the XRM 2011 Organization service exceptions (FaultException&lt;OrganizationServiceFault&gt;)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal static DataverseInteractionException NewExceptionByOrganizationServiceFault(string message, FaultException<OrganizationServiceFault> e)
    {
        return new DataverseInteractionException($"{message} e.Detail.Innerfault = {e.Detail?.InnerFault?.Message}, e.Detail.ErrorCode = 0x{e.Detail?.ErrorCode}, e.Detail.Message = {e.Detail?.Message}, e.Detail.Innerfault.Innerfault = {e.Detail?.InnerFault?.InnerFault?.Message}", e);
    }

    /// <summary>
    /// Formats the errormessage for a generic WCF FaultException
    /// </summary>
    /// <param name="message"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal static DataverseInteractionException NewExceptionByFaultException(string message, FaultException e)
    {
        return new DataverseInteractionException($"{message} Unknown WCF exception thrown. - e.Message = {e.Message}, stacktrace = {e.StackTrace}", e);
    }

    internal static DataverseInteractionException NewExceptionByDefaultException(string errorMsg, System.Exception e)
    {
        return new DataverseInteractionException(errorMsg + " Details: " + e.Message, e);
    }

    #endregion Private methods
}