using Azure.DataverseService.Foundation.Dao;
using DataverseService;
using DataverseService.EconomyArea;
using DataverseService.Foundation.Dao;
using DataverseService.UtilityArea;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using NSubstitute.ClearExtensions;
using SharedTest;

namespace Azure.DataverseService.Tests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly IOrganizationServiceAsync2 orgAdminService;
    private readonly IDataverseAccessObjectAsync adminDao;
    private readonly XrmMockup365 xrm;
    private readonly DataProducer dataProducer;
    private readonly IDataverseInvoiceService dataverseActivityService;
    private readonly IDataverseImageService dataverseImageService;
    private readonly IDataverseCustomApiService dataverseCustomApiService;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected IOrganizationService OrgAdminService => orgAdminService;

    protected IDataverseImageService DataverseImageService => dataverseImageService;

    protected DataProducer Producer => dataProducer;

    protected IDataverseInvoiceService DataverseActivityService => dataverseActivityService;

    public TestBase(XrmMockupFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = fixture.Xrm;
        orgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var dataverseAccessObjectAsyncLogger = loggerFactory.CreateLogger<DataverseAccessObjectAsync>();
        adminDao = new DataverseAccessObjectAsync(orgAdminService, dataverseAccessObjectAsyncLogger);
        dataProducer = new DataProducer(adminDao);
        dataverseCustomApiService = Substitute.For<IDataverseCustomApiService>();

        dataverseImageService = new DataverseImageService(adminDao);
        dataverseActivityService = new DataverseInvoiceService(adminDao, dataverseCustomApiService);
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
            dataverseCustomApiService.ClearSubstitute();
            Xrm.ResetEnvironment();
        }
    }
}