using Dataverse.Plugins;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain;
using SharedTest;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Dataverse.PluginTests;

public class XrmMockupFixture : IDisposable
{
    public const string SnapshotName = "ConfigBase";

    public XrmMockup365 Xrm { get; private set; }

    public DataverseAccessObject AdminDao { get; private set; }

    public IOrganizationService OrgAdminService { get; private set; }

    public DataProducer Producer { get; private set; }

    public WireMockServer Server { get; private set; }

    public XrmMockupFixture()
    {
        var settings = new XrmMockupSettings
        {
            BasePluginTypes = new Type[] { typeof(Plugin) },
            EnableProxyTypes = true,
            IncludeAllWorkflows = false,
            MetadataDirectoryPath = "..\\..\\..\\..\\..\\..\\SharedTest\\MetadataGenerated",
            ExceptionFreeRequests = new string[] { "AddToQueue" },
        };

        Xrm = XrmMockup365.GetInstance(settings);

        OrgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger("test");
        AdminDao = new DataverseAccessObject(OrgAdminService, logger);
        Producer = new DataProducer(AdminDao);
        Server = WireMockServer.Start();

        AddQueueEndpoints(new List<string>
        {
            QueueNames.DemoAccountQueue,
        });

        Xrm.TakeSnapshot(SnapshotName);
    }

    private void AddQueueEndpoints(IEnumerable<string> queuenames)
    {
        foreach (var queuename in queuenames)
        {
            Server
                .Given(Request.Create().WithPath($"/{queuename}/messages").UsingPost())
                .AtPriority(100)
                .RespondWith(
                Response.Create()
                .WithStatusCode(System.Net.HttpStatusCode.Created));
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
            Server?.Dispose();
        }
    }
}