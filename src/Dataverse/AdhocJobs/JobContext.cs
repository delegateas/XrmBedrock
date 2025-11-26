using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace AdhocJobs;

public record JobContext(
    IOrganizationService OrgService,
    DataverseAccessObject Dao,
    Uri DataverseUri,
    IServiceProvider Services);
