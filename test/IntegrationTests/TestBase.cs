using Azure.DataverseService.Foundation.Dao;
using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain;
using SharedTest;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.Server;
using XrmBedrock.SharedContext;

namespace IntegrationTests;

public class TestBase : IClassFixture<XrmMockupFixture>, IDisposable
{
    private readonly XrmMockup365 xrm;
    private readonly IDataverseAccessObjectAsync adminDao;
    private readonly DataProducer producer;
    private readonly MessageExecutor messageExecutor;
    private readonly WireMockServer server;
    private readonly IDataverseAccessObject userDao;
    private readonly Guid userIdOfUserDao;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected DataProducer Producer => producer;

    protected MessageExecutor MessageExecutor => messageExecutor;

    protected WireMockServer Server => server;

    /// <summary>
    /// This is for testing stuff that depends on the user context
    /// </summary>
    protected IDataverseAccessObject UserDao => userDao;

    protected Guid UserIdOfUserDao => userIdOfUserDao;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = XrmMockup365.GetInstance(fixture.Settings);

        // Setting up a user DAO for testing stuff that depends on the user context
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<TestBase>();
        userIdOfUserDao = Guid.NewGuid();
        CreateUser(userIdOfUserDao, Xrm.RootBusinessUnit, SecurityRoles.SystemAdministrator);
        var userService = Xrm.CreateOrganizationService(userIdOfUserDao);
        userDao = new DataverseAccessObject(userService, logger);

        adminDao = new DataverseAccessObjectAsync(xrm.GetAdminService(), Substitute.For<ILogger>());
        producer = new DataProducer(AdminDao);
        messageExecutor = new MessageExecutor();
        server = WireMockServer.Start();

        AddQueueEndpoints(QueueNames.AllQueues);

        // Create any data needed for the tests
        var envVarDefinition = new EnvironmentVariableDefinition
        {
            SchemaName = "mgs_AzureStorageAccountUrl",
        };
        envVarDefinition.Id = AdminDao.Create(envVarDefinition);
        AdminDao.Create(new EnvironmentVariableValue
        {
            EnvironmentVariableDefinitionId = envVarDefinition.ToEntityReference(),
            Value = Server.Url,
        });
    }

    protected SystemUser CreateUser(Guid userId, EntityReference businessUnit, params Guid[] securityRoles)
    {
        return Xrm.CreateUser(Xrm.GetAdminService(), new SystemUser { Id = userId, BusinessUnitId = businessUnit }, securityRoles).ToEntity<SystemUser>();
    }

    protected void AddQueueEndpoints(IEnumerable<string> queuenames)
    {
        ArgumentNullException.ThrowIfNull(queuenames);

        foreach (var queuename in queuenames)
        {
            Server
                .Given(Request.Create().WithPath($"/{queuename}/messages").UsingPost())
                .AtPriority(100)
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithCallback(req =>
                {
                    MessageExecutor.StoreMessage(new AwaitingMessage(queuename, req.Body ?? string.Empty));
                    return new ResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.Created,
                    };
                }));
        }
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
            server.Dispose();
        }
    }
}