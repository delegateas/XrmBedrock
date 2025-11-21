using DataverseRegistration;
using DG.Tools.XrmMockup;

namespace IntegrationTests;

public class XrmMockupFixture
{
    private static readonly object SettingsLock = new object();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private static XrmMockupSettings sharedSettings;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

#pragma warning disable CA1822 // Mark members as static
    public XrmMockupSettings Settings => sharedSettings;
#pragma warning restore CA1822 // Mark members as static

    public XrmMockupFixture()
    {
        lock (SettingsLock)
        {
            if (sharedSettings == null)
            {
                sharedSettings = new XrmMockupSettings
                {
                    BasePluginTypes = new Type[] { typeof(Plugin) },
                    BaseCustomApiTypes = new Tuple<string, Type>[] { new("demo", typeof(CustomAPI)) },
                    EnableProxyTypes = true,
                    IncludeAllWorkflows = false,
                    MetadataDirectoryPath = "..\\..\\..\\..\\SharedTest\\MetadataGenerated",
                };
            }
        }
    }
}
