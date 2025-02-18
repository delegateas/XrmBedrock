using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using IntegrationTests;
using SharedTest;

namespace Azure.DataverseService.Tests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly XrmMockupFixture fixture;

    protected XrmMockup365 Xrm => fixture.Xrm;

    protected IDataverseAccessObjectAsync AdminDao => fixture.AdminDao;

    protected DataProducer Producer => fixture.Producer;

    protected MessageExecutor MessageExecutor => fixture.MessageExecutor;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

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
            Xrm.ResetEnvironment();
        }
    }
}