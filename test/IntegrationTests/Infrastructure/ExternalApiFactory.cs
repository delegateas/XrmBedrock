using DG.Tools.XrmMockup;
using ExternalApi.CustomerArea.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;

namespace IntegrationTests.Infrastructure;

public sealed class ExternalApiFactory : WebApplicationFactory<CreateSubscriptionRequest>
{
    private readonly XrmMockup365 xrm;

    public WireMockServer WireMockServer { get; }

    public ExternalApiFactory(XrmMockup365 xrm, WireMockServer wireMockServer)
    {
        this.xrm = xrm;
        WireMockServer = wireMockServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.ReplaceDataverseServices(xrm);
        });
    }
}
