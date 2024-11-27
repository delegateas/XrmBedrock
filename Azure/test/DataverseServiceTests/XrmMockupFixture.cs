using DG.Tools.XrmMockup;

namespace LF.Medlemssystem.DataverseTests
{
    public class XrmMockupFixture
    {
        private readonly XrmMockup365 xrm;

        public XrmMockup365 Xrm => xrm;

        public XrmMockupFixture()
        {
            var settings = new XrmMockupSettings
            {
                BasePluginTypes = Array.Empty<Type>(),
                EnableProxyTypes = true,
                IncludeAllWorkflows = false,
                MetadataDirectoryPath = "..\\..\\..\\..\\..\\..\\SharedTest\\MetadataGenerated",
            };

            xrm = XrmMockup365.GetInstance(settings);
        }
    }
}