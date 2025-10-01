using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Tooling.Connector;
using SharedContext.Dao;
using System.Configuration;

namespace ConsoleJobs.Setup;

internal sealed class EnvironmentConfig
{
    public Environment CurrentEnvironment { get; private set; }

    public DataverseAccessObject Dao { get; private set; } = null!;

    public ILogger Tracing { get; private set; }

    /// <summary>
    /// By default set to the desktop folder.
    /// </summary>
    public string CsvFolderPath { get; private set; } = string.Empty;

    private EnvironmentConfig()
    {
        Tracing = new ConsoleLogger();
    }

    public static EnvironmentConfig Create(Environment env)
    {
        return new EnvironmentConfig
        {
            CurrentEnvironment = env,
            Dao = GetDao(env),
            CsvFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
        };
    }

    private static DataverseAccessObject GetDao(Environment env)
    {
        var connString = GetDataverseConnectionstring(env);
#pragma warning disable CA2000 // Dispose objects before losing scope
        var orgService = GetOrgService(connString);
#pragma warning restore CA2000 // Dispose objects before losing scope
        var logger = new ConsoleLogger();
        return new DataverseAccessObject(orgService, logger);
    }

    private static string GetDataverseConnectionstring(Environment env)
    {
        var auth = ConfigurationManager.AppSettings["AuthType"];
        var url = GetUrlFromEnvironment(env);
        var username = ConfigurationManager.AppSettings["Username"];
        var loginprompt = ConfigurationManager.AppSettings["LoginPrompt"];
        var appId = ConfigurationManager.AppSettings["AppId"];
        var redirectUri = ConfigurationManager.AppSettings["RedirectUri"];

        return $"AuthType={auth};url={url};Username={username};LoginPrompt={loginprompt};AppId={appId};RedirectUri={redirectUri};";
    }

    private static string GetUrlFromEnvironment(Environment env)
    {
        return env switch
        {
            Environment.Dev => ConfigurationManager.AppSettings["DevEnv"],
            Environment.Test => ConfigurationManager.AppSettings["TestEnv"],
            Environment.Uat => ConfigurationManager.AppSettings["UatEnv"],
            Environment.Prod => ConfigurationManager.AppSettings["ProdEnv"],
            _ => throw new ArgumentException("Environment not supported", nameof(env)),
        };
    }

    private static CrmServiceClient GetOrgService(string dataverseConnectionstring)
    {
        var client = new CrmServiceClient(dataverseConnectionstring);
        if (client.LastCrmException != null)
#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable S112 // General or reserved exceptions should never be thrown
            throw new Exception($"Connection to D365 fails with exception {client.LastCrmException.Message} Error: {client.LastCrmError}", client.LastCrmException);
#pragma warning restore S112 // General or reserved exceptions should never be thrown
#pragma warning restore CA2201 // Do not raise reserved exception types
        return client;
    }
}