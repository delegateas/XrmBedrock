using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System.Runtime.Caching;

namespace SharedContext.Dao;

/// <summary>
/// The DataverseAccessObject class wraps the IOrganizationService and provides a set of methods to interact with Dataverse.
/// These mathods provide a unified way of accessing Dataverse with systematic logging, stopwatch timing of operations and options for caching.
/// It also facilitates the dependency injection of the IOrganizationService with options to run as current user (plugins) or as an admin.
///
/// This class is implemented as partial to split an otherwise large file into smaller files.
/// </summary>
public partial class DataverseAccessObject : IDataverseAccessObject
{
    private readonly IOrganizationService orgService;
    protected readonly ILogger logger;
    private readonly CacheHandler cacheHandler;

    public DataverseAccessObject(IOrganizationService orgService, ILogger logger)
    {
        this.orgService = orgService;
        this.logger = logger;
        cacheHandler = new CacheHandler(MemoryCache.Default, logger);
    }
}