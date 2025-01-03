using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedTest;
using WireMock.Logging;
using WireMock.Server;
using XrmBedrock.SharedContext;

namespace Dataverse.PluginTests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly XrmMockupFixture fixture;
    private readonly IDataverseAccessObject userDao;
    private readonly Guid userIdOfUserDao;

    protected XrmMockup365 Xrm => fixture.Xrm;

    protected DataverseAccessObject AdminDao => fixture.AdminDao;

    private IOrganizationService OrgAdminService => fixture.OrgAdminService;

    protected DataProducer Producer => fixture.Producer;

    protected IEnumerable<ILogEntry> LogEntries => Server.LogEntries;

    protected WireMockServer Server => fixture.Server;

    /// <summary>
    /// This is for testing stuff that depends on the user context
    /// </summary>
    protected IDataverseAccessObject UserDao => userDao;

    protected Guid UserIdOfUserDao => userIdOfUserDao;

    public TestBase(XrmMockupFixture fixture)
    {
        if (fixture == null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        this.fixture = fixture;

        // Setting up a user DAO for testing stuff that depends on the user context
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<TestBase>();
        userIdOfUserDao = Guid.NewGuid();
        CreateUser(userIdOfUserDao, Xrm.RootBusinessUnit, SecurityRoles.SystemAdministrator);
        var userService = Xrm.CreateOrganizationService(userIdOfUserDao);
        userDao = new DataverseAccessObject(userService, logger);
    }

    protected SystemUser CreateUser(Guid userId, EntityReference businessUnit, params Guid[] securityRoles)
    {
        return Xrm.CreateUser(OrgAdminService, new SystemUser { Id = userId, BusinessUnitId = businessUnit }, securityRoles).ToEntity<SystemUser>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            Server.ResetLogEntries();
            Xrm.RestoreToSnapshot(XrmMockupFixture.SnapshotName);
        }
    }
}