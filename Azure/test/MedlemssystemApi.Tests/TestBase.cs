using DataverseService.ActivityArea;
using DataverseService.Foundation.Dao;
using DataverseService.UtilityArea;
using DG.Tools.XrmMockup;
using LF.Services.Dataverse.Dao;
using MedlemssystemApi.BusinessLogic;
using Microsoft.Extensions.Logging;
using SharedDataverseLogic.ActivityArea;
using SharedDomain;
using SharedTest;

namespace MedlemssystemApi.Tests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly IDataverseAccessObjectAsync adminDao;
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable CS0649 // Field 'TestBase.dataverseImageService' is never assigned to, and will always have its default value null
    private readonly IDataverseImageService dataverseImageService;
#pragma warning restore CS0649 // Field 'TestBase.dataverseImageService' is never assigned to, and will always have its default value null
#pragma warning restore S3459 // Unassigned members should be removed
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable CS0649 // Field 'TestBase.dataverselogger' is never assigned to, and will always have its default value null
    private readonly ILogger<DataverseActivityService> dataverselogger;
#pragma warning restore CS0649 // Field 'TestBase.dataverselogger' is never assigned to, and will always have its default value null
#pragma warning restore S3459 // Unassigned members should be removed
    private readonly XrmMockup365 xrm;
    private readonly DataProducer dataProducer;
    private readonly IActivityBusinessLogic activityBusinessLogic;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected IDataverseImageService DataverseImageService => dataverseImageService;

    protected IDataverseImageService I => dataverseImageService;

    protected ILogger<DataverseActivityService> DataverseLogger => dataverselogger;

    protected DataProducer Producer => dataProducer;

    protected IActivityBusinessLogic ActivityBusinessLogicInstance => activityBusinessLogic;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TestBase(XrmMockupFixture fixture)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = fixture.Xrm;
        var simpleLogger = new SimpleLogger<TestBase>();
        var loggingComponent = new LoggingComponent();
        loggingComponent.SetLogger(simpleLogger);
        var orgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var dataverseAccessObjectAsyncLogger = loggerFactory.CreateLogger<DataverseAccessObjectAsync>();
        adminDao = new DataverseAccessObjectAsync(orgAdminService, dataverseAccessObjectAsyncLogger);
        dataProducer = new DataProducer(adminDao);
#pragma warning disable CS8604 // Possible null reference argument.
        activityBusinessLogic = new ActivityBusinessLogic(
            new SharedDataverseActivityService(adminDao),
            loggingComponent);
#pragma warning restore CS8604 // Possible null reference argument.
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