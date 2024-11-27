using Azure.DataverseService.Foundation.Dao;
using DataverseService.ActivityArea;
using DataverseService.CustomerArea;
using DataverseService.Foundation.Dao;
using DataverseService.UtilityArea;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using SharedTest;

namespace Azure.DataverseService.Tests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly IOrganizationServiceAsync2 orgAdminService;
    private readonly IDataverseAccessObjectAsync adminDao;
    private readonly XrmMockup365 xrm;
    private readonly DataProducer dataProducer;
    private readonly IDataverseAccountService dataverseAccountService;
    private readonly IDataverseActivityService dataverseActivityService;
    private readonly IDataverseImageService dataverseImageService;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected IOrganizationService OrgAdminService => orgAdminService;

    protected IDataverseImageService DataverseImageService => dataverseImageService;

    protected DataProducer Producer => dataProducer;

    protected IDataverseAccountService DataverseAccountService => dataverseAccountService;

    protected IDataverseActivityService DataverseActivityService => dataverseActivityService;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = fixture.Xrm;
        orgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var dataverseAccessObjectAsyncLogger = loggerFactory.CreateLogger<DataverseAccessObjectAsync>();
        adminDao = new DataverseAccessObjectAsync(orgAdminService, dataverseAccessObjectAsyncLogger);
        dataProducer = new DataProducer(adminDao);
        dataverseImageService = new DataverseImageService(adminDao);
        dataverseAccountService = new DataverseAccountService(adminDao);
        dataverseActivityService = new DataverseActivityService(adminDao);
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