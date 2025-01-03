using Azure.DataverseService.Foundation.Dao;
using DataverseService.ActivityArea;
using DataverseService.Foundation.Dao;
using DemoExternalApi.BusinessLogic;
using DG.Tools.XrmMockup;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedDataverseLogic.ActivityArea;
using SharedTest;

namespace DemoExternalApi.Tests;

[Collection("Xrm Collection")]
public class TestBase : IDisposable
{
    private readonly IOrganizationServiceAsync2 orgAdminService;
    private readonly IDataverseAccessObjectAsync adminDao;
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable CS0649 // Field 'TestBase.dataverseImageService' is never assigned to, and will always have its default value null
    private readonly ILogger<DataverseActivityService> dataverselogger;
#pragma warning restore CS0649 // Field 'TestBase.dataverselogger' is never assigned to, and will always have its default value null
#pragma warning restore S3459 // Unassigned members should be removed
    private readonly XrmMockup365 xrm;
    private readonly DataProducer dataProducer;
    private readonly IActivityBusinessLogic activityBusinessLogic;

    protected XrmMockup365 Xrm => xrm;

    protected IDataverseAccessObjectAsync AdminDao => adminDao;

    protected ILogger DataverseLogger => dataverselogger;

    protected DataProducer Producer => dataProducer;

    protected IActivityBusinessLogic ActivityBusinessLogicInstance => activityBusinessLogic;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TestBase(XrmMockupFixture fixture)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        ArgumentNullException.ThrowIfNull(fixture);

        xrm = fixture.Xrm;

        // setting up the logger
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<TestBase>();

        // setting up the dataverse access objects and Producer
        orgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
        adminDao = new DataverseAccessObjectAsync(orgAdminService, logger);
        dataProducer = new DataProducer(adminDao);

        // setting up other services
#pragma warning disable CS8604 // Possible null reference argument.
        activityBusinessLogic = new ActivityBusinessLogic(
            new SharedDataverseActivityService(adminDao),
            logger);
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