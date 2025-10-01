using Azure.DataverseService.Foundation.Dao;
using Dataverse.Plugins;
using DG.Tools.XrmMockup;
using SharedDomain;
using SharedTest;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.Server;
using XrmBedrock.SharedContext;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IntegrationTests;

public class XrmMockupFixture
{
    public const string SnapshotName = "ConfigBase";

    private readonly XrmMockup365 xrm;

    public XrmMockup365 Xrm => xrm;

    public DataverseAccessObjectAsync AdminDao { get; private set; }

    public DataProducer Producer { get; private set; }

    public WireMockServer Server { get; private set; }

    public MessageExecutor MessageExecutor { get; private set; }

    public XrmMockupFixture()
    {
        var settings = new XrmMockupSettings
        {
            BasePluginTypes = new Type[] { typeof(Plugin) },
            BaseCustomApiTypes = new Tuple<string, Type>[] { new("mgs", typeof(CustomAPI)) },
            EnableProxyTypes = true,
            IncludeAllWorkflows = false,
            MetadataDirectoryPath = "..\\..\\..\\..\\SharedTest\\MetadataGenerated",
        };

        xrm = XrmMockup365.GetInstance(settings);
        AdminDao = new DataverseAccessObjectAsync(xrm.GetAdminService(), Substitute.For<ILogger>());
        Producer = new DataProducer(AdminDao);
        MessageExecutor = new MessageExecutor(AdminDao);
        Server = WireMockServer.Start();

        // Make sure all queues are added here
        AddQueueEndpoints(new List<string>
        {
            QueueNames.CreateInvoicesQueue,
        });

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

        Xrm.TakeSnapshot(SnapshotName);
    }

    /// <summary>
    /// Catches any messages send to the queues and stores them in the MessageExecutor
    /// </summary>
    private void AddQueueEndpoints(IEnumerable<string> queuenames)
    {
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
}