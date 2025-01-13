using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
// StepConfig           : className, ExecutionStage, EventOperation, LogicalName
// ExtendedStepConfig   : Deployment, ExecutionMode, Name, ExecutionOrder, FilteredAttributes, UserContext
// ImageTuple           : Name, EntityAlias, ImageType, Attributes
using StepConfig = System.Tuple<string, int, string, string>;
using ExtendedStepConfig = System.Tuple<int, int, string, int, string, string>;
using ImageTuple = System.Tuple<string, string, int, string>;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using DataverseLogic.Azure;
using XrmBedrock.SharedContext;
using Microsoft.Extensions.Logging;
using DataverseLogic;
using System.ServiceModel.Configuration;
using SharedDataverseLogic;
using SharedDomain;

namespace Dataverse.Plugins;

/// <summary>
/// Base class for all Plugins.
/// </summary>    
public class Plugin : IPlugin
{
    private Collection<Tuple<int, string, string, Action<IServiceProvider>>> registeredEvents;

    /// <summary>
    /// Gets the List of events that the plug-in should fire for. Each List
    /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
    /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
    /// </summary>
    protected Collection<Tuple<int, string, string, Action<IServiceProvider>>> RegisteredEvents
    {
        get
        {
            if (this.registeredEvents == null)
            {
                this.registeredEvents = new Collection<Tuple<int, string, string, Action<IServiceProvider>>>();
            }
            return this.registeredEvents;
        }
    }

    /// <summary>
    /// Gets or sets the name of the child class.
    /// </summary>
    /// <value>The name of the child class.</value>
    protected string ChildClassName
    {
        get;
        private set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="childClassName">The <see cref="" cred="Type"/> of the derived class.</param>
    internal Plugin() : this(null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="childClassName"></param>
    /// <param name="unsecure"></param>
    internal Plugin(string unsecure) : this(unsecure, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="childClassName"></param>
    /// <param name="unsecure"></param>
    /// <param name="secure"></param>
    internal Plugin(string unsecure, string secure)
    {
        this.ChildClassName = this.GetType().ToString();
    }

    /// <summary>
    /// Executes the plug-in.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <remarks>
    /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
    /// The plug-in's Execute method should be written to be stateless as the constructor 
    /// is not called for every invocation of the plug-in. Also, multiple system threads 
    /// could execute the plug-in at the same time. All per invocation state information 
    /// is stored in the context. This means that you should not use global variables in plug-ins.
    /// </remarks>
    public void Execute(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException("serviceProvider");
        }

        // Get services of the ServiceProvider
        var tracingService = serviceProvider.GetService<ITracingService>() ?? throw new Exception("Unable to get Tracing service");
        var pluginTelemetryLogger = serviceProvider.GetService<Microsoft.Xrm.Sdk.PluginTelemetry.ILogger>();
        var extendedTracingService = new ExtendedTracingService(tracingService, pluginTelemetryLogger);
        if (pluginTelemetryLogger == null)
            extendedTracingService.Trace("Unable to get PluginTelemetryLogger");
        extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));
        var context = serviceProvider.GetService<IPluginExecutionContext>() ?? throw new Exception("Unable to get PluginBase Execution Context");
        var organizationServiceFactory = serviceProvider.GetService<IOrganizationServiceFactory>() ?? throw new Exception("Unable to get service factory");
        var managedIdentity = serviceProvider.GetService<IManagedIdentityService>() ?? new DummyManagedIdentityService();

        // Create a new service collection and add the relevant services
        var services = new ServiceCollection();
        services.AddScoped(x => context);
        services.AddScoped(x => tracingService);
        services.AddScoped<IExtendedTracingService>(x => extendedTracingService);
        services.AddScoped(x => organizationServiceFactory);
        services.AddScoped(x => managedIdentity);
        services.AddScoped<ILogger, DataverseLogger>();
        services.TryAdd(ServiceDescriptor.Scoped(typeof(ILogger<>), typeof(DataverseLogger<>)));

        services.SetupCustomDependencies();

        // Build the service provider
        var localServiceProvider = services.BuildServiceProvider();
        try
        {
            // Iterate over all of the expected registered events to ensure that the plugin
            // has been invoked by an expected event
            // For any given plug-in event at an instance in time, we would expect at most 1 result to match.
            Action<IServiceProvider> entityAction =
                (from a in this.RegisteredEvents
                 where (
                 a.Item1 == context.Stage &&
                 a.Item2 == context.MessageName &&
                 (string.IsNullOrWhiteSpace(a.Item3) ? true : a.Item3 == context.PrimaryEntityName)
                 )
                 select a.Item4).FirstOrDefault();
            if (entityAction != null)
            {
                extendedTracingService.Trace(string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} is firing for Entity: {1}, Message: {2}",
                    this.ChildClassName,
                    context.PrimaryEntityName,
                    context.MessageName));
                entityAction.Invoke(localServiceProvider);
                // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                // guard against multiple executions.
                return;
            }
        }
        catch (FaultException<OrganizationServiceFault> e)
        {
            extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
            throw new InvalidPluginExecutionException(OperationStatus.Failed, e.Message);
        }
        catch (NotImplementedException e)
        {
            extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
            throw new InvalidPluginExecutionException(OperationStatus.Failed, e.Message);
        }
        finally
        {
            extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
        }
    }

    /// <summary>
    /// The methods exposes the RegisteredEvents as a collection of tuples
    /// containing:
    /// - The full assembly name of the class containing the RegisteredEvents
    /// - The Pipeline Stage
    /// - The Event Operation
    /// - Logical Entity Name (or empty for all)
    /// This will allow to instantiate each plug-in and iterate through the 
    /// PluginProcessingSteps in order to sync the code repository with 
    /// MS XRM without have to use any extra layer to perform this operation
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Tuple<string, int, string, string>> PluginProcessingSteps()
    {
        var className = this.ChildClassName;
        foreach (var events in this.RegisteredEvents)
        {
            yield return new Tuple<string, int, string, string>
                (className, events.Item1, events.Item2, events.Item3);
        }
    }

    #region Additional helper methods

    protected static bool MatchesEventOperation(IPluginExecutionContext context, params EventOperation[] operations)
    {
        var operation = context.MessageName.ToEventOperation();
        return operations.Any(o => o == operation);
    }


    #endregion

    #region PluginStepConfig retrieval
    /// <summary>
    /// Made by Delegate A/S
    /// Get the plugin step configurations.
    /// </summary>
    /// <returns>List of steps</returns>
    public IEnumerable<Tuple<StepConfig, ExtendedStepConfig, IEnumerable<ImageTuple>>> PluginProcessingStepConfigs()
    {
        var className = this.ChildClassName;
        foreach (var config in this.PluginStepConfigs)
        {
            yield return
                new Tuple<StepConfig, ExtendedStepConfig, IEnumerable<ImageTuple>>(
                    new StepConfig(className, config._ExecutionStage, config._EventOperation, config._LogicalName),
                    new ExtendedStepConfig(config._Deployment, config._ExecutionMode, config._Name, config._ExecutionOrder, config._FilteredAttributes, config._UserContext.ToString()),
                    config.GetImages());
        }
    }


    protected PluginStepConfig<E> RegisterPluginStep<E>(
        EventOperation eventOperation, ExecutionStage executionStage, Action<IServiceProvider> action) where E : Entity
    {
        PluginStepConfig<E> stepConfig = new PluginStepConfig<E>(eventOperation, executionStage);
        this.PluginStepConfigs.Add((IPluginStepConfig)stepConfig);

        this.RegisteredEvents.Add(
            new Tuple<int, string, string, Action<IServiceProvider>>(
                stepConfig._ExecutionStage,
                stepConfig._EventOperation,
                stepConfig._LogicalName,
                action));

        return stepConfig;
    }


    private Collection<IPluginStepConfig> pluginConfigs;
    private Collection<IPluginStepConfig> PluginStepConfigs
    {
        get
        {
            if (this.pluginConfigs == null)
            {
                this.pluginConfigs = new Collection<IPluginStepConfig>();
            }
            return this.pluginConfigs;
        }
    }
    #endregion
}

#region PluginStepConfig made by Delegate A/S
public static class HelperPlugin
{
    public static EventOperation ToEventOperation(this String x)
    {
        return (EventOperation)Enum.Parse(typeof(EventOperation), x);
    }
}

interface IPluginStepConfig
{
    string _LogicalName { get; }
    string _EventOperation { get; }
    int _ExecutionStage { get; }

    string _Name { get; }
    int _Deployment { get; }
    int _ExecutionMode { get; }
    int _ExecutionOrder { get; }
    string _FilteredAttributes { get; }
    Guid _UserContext { get; }
    IEnumerable<ImageTuple> GetImages();
}

/// <summary>
/// Made by Delegate A/S
/// Class to encapsulate the various configurations that can be made 
/// to a plugin step.
/// </summary>
public class PluginStepConfig<T> : IPluginStepConfig where T : Entity
{
    public string _LogicalName { get; private set; }
    public string _EventOperation { get; private set; }
    public int _ExecutionStage { get; private set; }

    public string _Name { get; private set; }
    public int _Deployment { get; private set; }
    public int _ExecutionMode { get; private set; }
    public int _ExecutionOrder { get; private set; }
    public Guid _UserContext { get; private set; }

    public Collection<PluginStepImage> _Images = new Collection<PluginStepImage>();
    public Collection<string> _FilteredAttributesCollection = new Collection<string>();

    public string _FilteredAttributes
    {
        get
        {
            if (this._FilteredAttributesCollection.Count == 0) return null;
            return string.Join(",", this._FilteredAttributesCollection).ToLower();
        }
    }


    public PluginStepConfig(EventOperation eventOperation, ExecutionStage executionStage)
    {
        this._LogicalName = Activator.CreateInstance<T>().LogicalName;
        this._EventOperation = eventOperation.ToString();
        this._ExecutionStage = (int)executionStage;
        this._Deployment = (int)Deployment.ServerOnly;
        this._ExecutionMode = (int)ExecutionMode.Synchronous;
        this._ExecutionOrder = 1;
        this._UserContext = Guid.Empty;
    }

    private PluginStepConfig<T> AddFilteredAttribute(Expression<Func<T, object>> lambda)
    {
        this._FilteredAttributesCollection.Add(GetMemberName(lambda));
        return this;
    }

    public PluginStepConfig<T> AddFilteredAttributes(params Expression<Func<T, object>>[] lambdas)
    {
        foreach (var lambda in lambdas) this.AddFilteredAttribute(lambda);
        return this;
    }

    public PluginStepConfig<T> SetDeployment(Deployment deployment)
    {
        this._Deployment = (int)deployment;
        return this;
    }

    public PluginStepConfig<T> SetExecutionMode(ExecutionMode executionMode)
    {
        this._ExecutionMode = (int)executionMode;
        return this;
    }

    public PluginStepConfig<T> SetName(string name)
    {
        this._Name = name;
        return this;
    }

    public PluginStepConfig<T> SetExecutionOrder(int executionOrder)
    {
        this._ExecutionOrder = executionOrder;
        return this;
    }

    public PluginStepConfig<T> SetUserContext(Guid userContext)
    {
        this._UserContext = userContext;
        return this;
    }

    public PluginStepConfig<T> AddImage(ImageType imageType)
    {
        return this.AddImage(imageType, null);
    }

    public PluginStepConfig<T> AddImage(ImageType imageType, params Expression<Func<T, object>>[] attributes)
    {
        return this.AddImage(imageType.ToString(), imageType.ToString(), imageType, attributes);
    }

    public PluginStepConfig<T> AddImage(string name, string entityAlias, ImageType imageType)
    {
        return this.AddImage(name, entityAlias, imageType, null);
    }

    public PluginStepConfig<T> AddImage(string name, string entityAlias, ImageType imageType, params Expression<Func<T, object>>[] attributes)
    {
        this._Images.Add(new PluginStepImage(name, entityAlias, imageType, attributes));
        return this;
    }

    public IEnumerable<ImageTuple> GetImages()
    {
        foreach (var image in this._Images)
        {
            yield return new ImageTuple(image.Name, image.EntityAlias, image.ImageType, image.Attributes);
        }
    }

    /// <summary>
    /// Container for information about images attached to steps
    /// </summary>
    public class PluginStepImage
    {
        public string Name { get; private set; }
        public string EntityAlias { get; private set; }
        public int ImageType { get; private set; }
        public string Attributes { get; private set; }

        public PluginStepImage(string name, string entityAlias, ImageType imageType, Expression<Func<T, object>>[] attributes)
        {
            this.Name = name;
            this.EntityAlias = entityAlias;
            this.ImageType = (int)imageType;

            if (attributes != null && attributes.Length > 0)
            {
                this.Attributes = string.Join(",", attributes.Select(x => PluginStepConfig<T>.GetMemberName(x))).ToLower();
            }
            else
            {
                this.Attributes = null;
            }
        }
    }


    private static string GetMemberName(Expression<Func<T, object>> lambda)
    {
        MemberExpression body = lambda.Body as MemberExpression;

        if (body == null)
        {
            UnaryExpression ubody = (UnaryExpression)lambda.Body;
            body = ubody.Operand as MemberExpression;
        }

        return body.Member.Name;
    }
}

class AnyEntity : Entity
{
    public AnyEntity() : base("") { }
}

/**
 * Enums to help setup plugin steps
 */

public enum ExecutionMode
{
    Synchronous = 0,
    Asynchronous = 1,
}

public enum ExecutionStage
{
    PreValidation = 10,
    PreOperation = 20,
    PostOperation = 40,
}

public enum Deployment
{
    ServerOnly = 0,
    MicrosoftDynamicsCRMClientforOutlookOnly = 1,
    Both = 2,
}

// EventOperation based on CRM 2016
public enum EventOperation
{
    AddItem,
    AddListMembers,
    AddMember,
    AddMembers,
    AddPrincipalToQueue,
    AddPrivileges,
    AddProductToKit,
    AddRecurrence,
    AddToQueue,
    AddUserToRecordTeam,
    ApplyRecordCreationAndUpdateRule,
    Assign,
    AssignUserRoles,
    Associate,
    BackgroundSend,
    Book,
    CalculatePrice,
    Cancel,
    CheckIncoming,
    CheckPromote,
    Clone,
    CloneProduct,
    Close,
    CopyDynamicListToStatic,
    CopySystemForm,
    Create,
    CreateException,
    CreateInstance,
    CreateKnowledgeArticleTranslation,
    CreateKnowledgeArticleVersion,
    Delete,
    DeleteOpenInstances,
    DeliverIncoming,
    DeliverPromote,
    DetachFromQueue,
    Disassociate,
    Execute,
    ExecuteById,
    Export,
    ExportAll,
    ExportCompressed,
    ExportCompressedAll,
    GenerateSocialProfile,
    GetDefaultPriceLevel,
    GrantAccess,
    Handle,
    Import,
    ImportAll,
    ImportCompressedAll,
    ImportCompressedWithProgress,
    ImportWithProgress,
    LockInvoicePricing,
    LockSalesOrderPricing,
    Lose,
    Merge,
    ModifyAccess,
    PickFromQueue,
    Publish,
    PublishAll,
    PublishTheme,
    QualifyLead,
    Recalculate,
    ReleaseToQueue,
    RemoveFromQueue,
    RemoveItem,
    RemoveMember,
    RemoveMembers,
    RemovePrivilege,
    RemoveProductFromKit,
    RemoveRelated,
    RemoveUserFromRecordTeam,
    RemoveUserRoles,
    ReplacePrivileges,
    Reschedule,
    Retrieve,
    RetrieveExchangeRate,
    RetrieveFilteredForms,
    RetrieveMultiple,
    RetrievePersonalWall,
    RetrievePrincipalAccess,
    RetrieveRecordWall,
    RetrieveSharedPrincipalsAndAccess,
    RetrieveUnpublished,
    RetrieveUnpublishedMultiple,
    RetrieveUserQueues,
    RevokeAccess,
    Route,
    RouteTo,
    Send,
    SendFromTemplate,
    SetLocLabels,
    SetRelated,
    SetState,
    SetStateDynamicEntity,
    TriggerServiceEndpointCheck,
    UnlockInvoicePricing,
    UnlockSalesOrderPricing,
    Update,
    ValidateRecurrenceRule,
    Win
}

public enum ImageType
{
    PreImage = 0,
    PostImage = 1,
    Both = 2,
}
#endregion
