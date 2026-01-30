using Azure.DataverseService.Foundation.Dao;
using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using EconomyAreaWorker.Workers;
using IntegrationTests.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
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
    private readonly ExternalApiFactory apiFactory;
    private readonly WorkerFactory<CreateInvoicesWorker> workerFactory;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected DataProducer Producer => producer;

    protected MessageExecutor MessageExecutor => messageExecutor;

    protected WireMockServer Server => server;

    protected HttpClient ApiClient { get; }

    /// <summary>
    /// This is for testing stuff that depends on the user context
    /// </summary>
    protected IDataverseAccessObject UserDao => userDao;

    protected Guid UserIdOfUserDao => userIdOfUserDao;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = XrmMockup365.GetInstance(fixture.Settings);
        server = WireMockServer.Start();

        // Create API factory with test services
        apiFactory = new ExternalApiFactory(xrm, server);
        ApiClient = apiFactory.CreateClient();

        // Create Worker factory with test services (replaces FunctionsTestHost)
        workerFactory = new WorkerFactory<CreateInvoicesWorker>(xrm, server);

        // Initialize the factory to populate WorkerRegistrations by accessing Services
        _ = workerFactory.Services;

        // Create MessageExecutor with auto-discovered registrations
        messageExecutor = new MessageExecutor(workerFactory.Services, workerFactory.WorkerRegistrations);

        // Setting up a user DAO for testing stuff that depends on the user context
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<TestBase>();
        userIdOfUserDao = Guid.NewGuid();
        CreateUser(userIdOfUserDao, Xrm.RootBusinessUnit, SecurityRoles.SystemAdministrator);
        var userService = Xrm.CreateOrganizationService(userIdOfUserDao);
        userDao = new DataverseAccessObject(userService, logger);

        // Create DAO for test data setup (shares XrmMockup instance)
        adminDao = new DataverseAccessObjectAsync(xrm.GetAdminService(), Substitute.For<ILogger>());
        producer = new DataProducer(AdminDao);

        // Add queue endpoints using auto-discovered queue names from registrations
        AddQueueEndpoints(workerFactory.WorkerRegistrations.Select(r => r.QueueName));

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
            ApiClient.Dispose();
            apiFactory.Dispose();
            workerFactory.Dispose();
            server.Dispose();
        }
    }
}
