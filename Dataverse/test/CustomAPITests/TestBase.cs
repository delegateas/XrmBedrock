using Dataverse.Plugins;
using DataverseLogic;
using DG.Tools.XrmMockup;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedTest;

namespace Dataverse.CustomAPITests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly XrmMockupFixture fixture;

    protected XrmMockup365 Xrm => fixture.Xrm;

    protected IAdminDataverseAccessObjectService AdminDao { get; private set; }

    protected IOrganizationService OrgAdminService => fixture.OrgAdminService;

    protected DataProducer Producer => fixture.Producer;

    public TestBase(XrmMockupFixture fixture)
    {
        if (fixture == null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        this.fixture = fixture;
        var serviceFactory = Substitute.For<IOrganizationServiceFactory>();
        serviceFactory.CreateOrganizationService(null).ReturnsForAnyArgs(OrgAdminService);
        AdminDao = new AdminDataverseAccessObjectService(serviceFactory, new DataverseLogger(new VoidTracingService()));
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
            Xrm.RestoreToSnapshot(XrmMockupFixture.SnapshotName);
        }
    }
}