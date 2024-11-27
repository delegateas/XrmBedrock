using ActivityAreaFunctionApp.Tests;
using DataverseService.Foundation.Dao;
using DG.Tools.XrmMockup;
using LF.Services.Dataverse.Dao;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using SharedTest;

namespace LF.Medlemssystem.DataverseTests
{
    [Collection("Xrm Collection")]
    public class TestBase : IDisposable
    {
        private readonly IOrganizationServiceAsync2 orgAdminService;
        private readonly IDataverseAccessObjectAsync adminDao;
        private readonly XrmMockup365 xrm;
        private readonly DataProducer dataProducer;

        protected XrmMockup365 Xrm => xrm;

        protected IDataverseAccessObjectAsync AdminDao => adminDao;

        protected IOrganizationService OrgAdminService => orgAdminService;

        protected DataProducer Producer => dataProducer;

        public TestBase(XrmMockupFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            xrm = fixture.Xrm;
            orgAdminService = Xrm.GetAdminService(new MockupServiceSettings(true, false, MockupServiceSettings.Role.UI));
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
            var dataverseAccessObjectAsyncLogger = loggerFactory.CreateLogger<DataverseAccessObjectAsync>();
            adminDao = new DataverseAccessObjectAsync(orgAdminService, dataverseAccessObjectAsyncLogger);
            dataProducer = new DataProducer(adminDao);
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
}