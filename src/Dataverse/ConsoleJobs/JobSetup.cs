using Azure.Identity;
using DataverseConnection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace ConsoleJobs;

public static class JobSetup
{
    public static JobContext Initialize(string[] args)
    {
        return Initialize(args, null);
    }

    public static JobContext Initialize(string[] args, Action<IServiceCollection>? configureServices)
    {
        ArgumentNullException.ThrowIfNull(args);
        var dataverseUri = args.Length > 0 ? new Uri(args[0]) : throw new InvalidOperationException("Dataverse Url is needed as the first argument");

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddDataverseWithOrganizationServices(options =>
        {
            // Swap with whatever fits you. This uses the token from az login
            options.TokenCredential = new AzureCliCredential();
            options.DataverseUrl = dataverseUri.ToString();
        });

        configureServices?.Invoke(services);

        var sp = services.BuildServiceProvider();
        var orgService = sp.GetRequiredService<IOrganizationService>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("ConsoleJob");
        var dao = new DataverseAccessObject(orgService, logger);

        var ctx = new JobContext(orgService, dao, dataverseUri, sp, logger);

        logger.LogInformation($"Connected to {ctx.DataverseUri}");

        return ctx;
    }
}
