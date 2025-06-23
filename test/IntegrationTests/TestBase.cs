using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedTest;
using XrmBedrock.SharedContext;

namespace IntegrationTests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly XrmMockupFixture fixture;
    private readonly IDataverseAccessObject userDao;
    private readonly Guid userIdOfUserDao;

    protected XrmMockup365 Xrm => fixture.Xrm;

    protected IDataverseAccessObjectAsync AdminDao => fixture.AdminDao;

    protected DataProducer Producer => fixture.Producer;

    protected MessageExecutor MessageExecutor => fixture.MessageExecutor;

    /// <summary>
    /// This is for testing stuff that depends on the user context
    /// </summary>
    protected IDataverseAccessObject UserDao => userDao;

    protected Guid UserIdOfUserDao => userIdOfUserDao;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

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
        return Xrm.CreateUser(fixture.Xrm.GetAdminService(), new SystemUser { Id = userId, BusinessUnitId = businessUnit }, securityRoles).ToEntity<SystemUser>();
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
            Xrm.ResetEnvironment();
        }
    }
}