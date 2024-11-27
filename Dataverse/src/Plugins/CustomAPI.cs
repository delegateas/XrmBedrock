namespace DG.XrmFramework.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    // MainCustomAPIConfig      : UniqueName, IsFunction, EnabledForWorkflow, AllowedCustomProcessingStepType, BindingType, BoundEntityLogicalName
    // ExtendedCustomAPIConfig  : PluginType, OwnerId, OwnerType, IsCustomizable, IsPrivate, ExecutePrivilegeName, Description
    // RequestParameterConfig   : Name, UniqueName, DisplayName, IsCustomizable, IsOptional, LogicalEntityName, Type
    // ResponsePropertyConfig   : Name, UniqueName, DisplayName, IsCustomizable, LogicalEntityName, Type
    using MainCustomAPIConfig = System.Tuple<string, bool, int, int, int, string>;
    using ExtendedCustomAPIConfig = System.Tuple<string, string, string, bool, bool, string, string>;
    using RequestParameterConfig = System.Tuple<string, string, string, bool, bool, string, int>; // TODO: Add description maybe
    using ResponsePropertyConfig = System.Tuple<string, string, string, bool, string, int>; // TODO
    using LF.Medlemssystem.Plugins;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using DataverseLogic;
    using SharedDataverseLogic;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Base class for all CustomAPIs.
    /// </summary>    
    public class CustomAPI : IPlugin
    {
        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
        /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
        /// </summary>
        protected Action<IServiceProvider> RegisteredEvent { get; private set; }

        /// <summary>
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        protected string ChildClassName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAPI"/> class.
        /// </summary>
        /// <param name="childClassName">The <see cref="" cred="Type"/> of the derived class.</param>
        internal CustomAPI(Type childClassName)
        {
            this.ChildClassName = childClassName.ToString();
        }


        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics XRM caches plug-in instances. 
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

            var localServiceProvider = services.BuildServiceProvider();

            extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));

            try
            {
                if (this.RegisteredEvent != null)
                {
                    tracingService.Trace(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} is firing for Entity: {1}, Message: {2}",
                        this.ChildClassName,
                        context.PrimaryEntityName,
                        context.MessageName));

                    this.RegisteredEvent.Invoke(localServiceProvider);

                    // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                    // guard against multiple executions.
                    return;
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));

                // Handle the exception.
                throw;
            }
            finally
            {
                extendedTracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
            }
        }

        // Delegate A/S added:
        /// <summary>
        /// The methods exposes the RegisteredEvents as a collection of tuples
        /// containing:
        /// - The full assembly name of the class containing the RegisteredEvents
        /// - The Pipeline Stage
        /// - The Event Operation
        /// - Logical Entity Name (or empty for all)
        /// This will allow to instantiate each plug-in and iterate through the 
        /// PluginProcessingSteps in order to sync the code repository with 
        /// MS CRM without have to use any extra layer to perform this operation
        /// </summary>
        /// <returns></returns>
        /// 

        //public IEnumerable<Tuple<string, int, string, string>> PluginProcessingSteps()
        //{
        //    var className = this.ChildClassName;
        //    foreach (var events in this.RegisteredEvents)
        //    {
        //        yield return new Tuple<string, int, string, string>
        //            (className, events.Item1, events.Item2, events.Item3);
        //    }
        //}

        #region CustomAPI retrieval
        /// <summary>
        /// Made by Delegate A/S
        /// Get the CustomAPI configurations.
        /// </summary>
        /// <returns>API</returns>
        public Tuple<MainCustomAPIConfig, ExtendedCustomAPIConfig, IEnumerable<RequestParameterConfig>, IEnumerable<ResponsePropertyConfig>> GetCustomAPIConfig()
        {
            var className = this.ChildClassName;
            var config = this.CustomAPIConfig;
            return new Tuple<MainCustomAPIConfig, ExtendedCustomAPIConfig, IEnumerable<RequestParameterConfig>, IEnumerable<ResponsePropertyConfig>>(
                new MainCustomAPIConfig(config._Name, config._IsFunction, config._EnabledForWorkflow, config._AllowedCustomProcessingStepType, config._BindingType, config._BoundEntityLogicalName),
                new ExtendedCustomAPIConfig(className, "", "", config._IsCustomizable, config._IsPrivate, config._ExecutePrivilegeName, config._Description),
                config.GetRequestParameters(),
                config.GetResponseProperties());
        }


        protected CustomAPIConfig RegisterCustomAPI(string name, Action<IServiceProvider> action)
        {
            var apiConfig = new CustomAPIConfig(name);

            if (this.CustomAPIConfig != null || this.RegisteredEvent != null)
            {
                throw new InvalidOperationException("The CustomAPI class does not support multiple registrations");
            }
            this.CustomAPIConfig = (ICustomAPIConfig)apiConfig;
            this.RegisteredEvent = action;
            return apiConfig;
        }

        //private ICustomAPIConfig apiConfig;
        private ICustomAPIConfig CustomAPIConfig { get; set; }
        #endregion
    }

    #region CustomAPIConfig made by Delegate A/S
    interface ICustomAPIConfig
    {
        int _AllowedCustomProcessingStepType { get; }
        int _BindingType { get; }
        string _BoundEntityLogicalName { get; }
        string _Description { get; }
        string _DisplayName { get; }
        string _ExecutePrivilegeName { get; }
        bool _IsCustomizable { get; }
        bool _IsFunction { get; }
        bool _IsPrivate { get; }
        string _Name { get; }
        string _PluginType { get; }
        string _UniqueName { get; }
        int _EnabledForWorkflow { get; }
        IEnumerable<RequestParameterConfig> GetRequestParameters();
        IEnumerable<ResponsePropertyConfig> GetResponseProperties();
    }

    public class CustomAPIConfig : ICustomAPIConfig
    { // TODO
        public int _AllowedCustomProcessingStepType { get; private set; }
        public int _BindingType { get; private set; }
        public string _BoundEntityLogicalName { get; private set; }
        public string _Description { get; private set; } // Remove?
        public string _DisplayName { get; private set; } // Remove?
        public string _ExecutePrivilegeName { get; private set; }
        public bool _IsCustomizable { get; private set; }
        public bool _IsFunction { get; private set; }
        public bool _IsPrivate { get; private set; }
        public string _Name { get; private set; }
        public string _UniqueName { get; private set; } // TODO: Maybe Remove, could control internally
        public string _PluginType { get; private set; }
        public int _EnabledForWorkflow { get; private set; }

        public Collection<CustomAPIRequestParameter> _RequestParameters = new Collection<CustomAPIRequestParameter>();
        public Collection<CustomAPIResponseProperty> _ResponseProperties = new Collection<CustomAPIResponseProperty>();

        public CustomAPIConfig(string name)
        {
            this._Name = name;
            this._DisplayName = name;
            this._UniqueName = name;
            this._IsFunction = false;
            this._EnabledForWorkflow = 0;
            this._AllowedCustomProcessingStepType = 0; // None
            this._BindingType = 0; // Global
            this._BoundEntityLogicalName = "";

            this._PluginType = null;
            this._IsCustomizable = false;
            this._IsPrivate = false;
            this._ExecutePrivilegeName = null; // TODO
            this._Description = "Description"; // TODO Can not be empty
        }

        public CustomAPIConfig AllowCustomProcessingStep(AllowedCustomProcessingStepType type)
        {
            this._AllowedCustomProcessingStepType = (int)type;
            return this;
        }

        public CustomAPIConfig Bind<T>(BindingType bindingType) where T : Entity
        {
            this._BindingType = (int)bindingType;
            this._BoundEntityLogicalName = Activator.CreateInstance<T>().LogicalName;
            return this;
        }

        public CustomAPIConfig MakeFunction()
        {
            this._IsFunction = true;
            return this;
        }
        public CustomAPIConfig MakePrivate()
        {
            this._IsPrivate = true;
            return this;
        }

        public CustomAPIConfig EnableForWorkFlow()
        {
            this._EnabledForWorkflow = 1;
            return this;
        }

        public CustomAPIConfig EnableCustomization()
        {
            this._IsCustomizable = true;
            return this;
        }

        public CustomAPIConfig AddRequestParameter(CustomAPIRequestParameter reqParam)
        {
            reqParam.SetNameFromAPI(this._Name);
            this._RequestParameters.Add(reqParam);
            return this;
        }

        public CustomAPIConfig AddResponseProperty(CustomAPIResponseProperty respProperty)
        {
            respProperty.SetNameFromAPI(this._Name);
            this._ResponseProperties.Add(respProperty);
            return this;
        }

        public IEnumerable<RequestParameterConfig> GetRequestParameters()
        {
            foreach (var requestParameter in this._RequestParameters)
            {
                // TODO: Add description maybe
                yield return new RequestParameterConfig(
                    requestParameter._Name,
                    requestParameter._UniqueName,
                    requestParameter._DisplayName,
                    requestParameter._IsCustomizable,
                    requestParameter._IsOptional,
                    requestParameter._LogicalEntityName,
                    (int)requestParameter._Type
                    );
            }
        }

        public IEnumerable<ResponsePropertyConfig> GetResponseProperties()
        {
            foreach (var responseProperty in this._ResponseProperties)
            {
                yield return new ResponsePropertyConfig(
                    responseProperty._Name,
                    responseProperty._UniqueName,
                    responseProperty._DisplayName,
                    responseProperty._IsCustomizable,
                    responseProperty._LogicalEntityName,
                    (int)responseProperty._Type);
            }
        }

        /// <summary>
        /// Container for information about Request Parameters attached to Custom APIs
        /// </summary>
        public class CustomAPIRequestParameter
        {
            public string _Name { get; private set; }
            public string _UniqueName { get; private set; } // TODO: Maybe Remove, could control internally
            public string _Description { get; private set; } // TODO: Maybe Remove, could control internally
            public string _DisplayName { get; private set; } // TODO: Maybe Remove, could control internally
            public bool _IsCustomizable { get; private set; }
            public bool _IsOptional { get; private set; }
            public string _LogicalEntityName { get; private set; }
            public RequestParameterType _Type { get; private set; }

            public CustomAPIRequestParameter(
                string name, RequestParameterType type, bool isCustomizable = false, bool isOptional = false, string logicalEntityName = null)
            {
                this._UniqueName = name;
                this._IsCustomizable = isCustomizable;
                this._IsOptional = isOptional;
                this._LogicalEntityName = logicalEntityName;
                this._Type = type;
            }

            public void SetNameFromAPI(string apiName)
            {
                var name = $"{apiName}-In-{_UniqueName}";
                _Name = name;
                _DisplayName = name;
                _Description = name;
            }
        }

        /// <summary>
        /// Container for information about Response Properties attached to Custom APIs
        /// </summary>
        public class CustomAPIResponseProperty
        {
            public string _Name { get; private set; }
            public string _UniqueName { get; private set; } // TODO: Maybe Remove, could control internally
            public string _Description { get; private set; } // TODO: Maybe Remove, could control internally
            public string _DisplayName { get; private set; } // TODO: Maybe Remove, could control internally
            public bool _IsCustomizable { get; private set; }
            public string _LogicalEntityName { get; private set; }
            public RequestParameterType _Type { get; private set; }

            public CustomAPIResponseProperty(
                string name, RequestParameterType type, bool isCustomizable = false, string logicalEntityName = null)
            {
                this._UniqueName = name;
                this._IsCustomizable = isCustomizable;
                this._LogicalEntityName = logicalEntityName;
                this._Type = type;
            }

            public void SetNameFromAPI(string apiName)
            {
                var name = $"{apiName}-Out-{_UniqueName}";
                _Name = name;
                _DisplayName = name;
                _Description = name;
            }
        }
    }

    public enum RequestParameterType
    {
        Boolean = 0,
        DateTime = 1,
        Decimal = 2,
        Entity = 3,
        EntityCollection = 4,
        EntityReference = 5,
        Float = 6,
        Integer = 7,
        Money = 8,
        Picklist = 9,
        String = 10,
        StringArray = 11,
        Guid = 12
    }

    public enum AllowedCustomProcessingStepType
    {
        //None = 0, // This value is default and should not be selectable
        AsyncOnly = 1,
        SyncAndAsync = 2
    }

    public enum BindingType
    {
        // Global = 0, // This value is default and should not be selectable
        Entity = 1,
        EntityCollection = 2
    }
    #endregion
}
