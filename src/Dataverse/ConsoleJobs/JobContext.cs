using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace ConsoleJobs;

public record JobContext(
    IOrganizationService OrgService,
    DataverseAccessObject Dao,
    Uri DataverseUri,
    IServiceProvider Services,
    ILogger Logger);
