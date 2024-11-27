using DG.Tools.XrmMockup;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedTest;
using WireMock.Logging;
using WireMock.Server;

namespace Dataverse.PluginTests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly XrmMockupFixture fixture;

    protected XrmMockup365 Xrm => fixture.Xrm;

    protected DataverseAccessObject AdminDao => fixture.AdminDao;

    protected IOrganizationService OrgAdminService => fixture.OrgAdminService;

    protected DataProducer Producer => fixture.Producer;

    protected IEnumerable<ILogEntry> LogEntries => Server.LogEntries;

    protected WireMockServer Server => fixture.Server;

    public TestBase(XrmMockupFixture fixture)
    {
        if (fixture == null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        this.fixture = fixture;
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