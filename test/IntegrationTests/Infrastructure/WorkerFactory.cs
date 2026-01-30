using Azure.Storage.Queues;
using DataverseService.Workers;
using DG.Tools.XrmMockup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using WireMock.Server;

namespace IntegrationTests.Infrastructure;

/// <summary>
/// Generic WebApplicationFactory for worker applications that replaces Dataverse services with XrmMockup
/// and removes the background services to prevent automatic queue polling during tests.
/// Uses auto-discovery from WorkerRegistration to expose registered workers.
/// </summary>
/// <typeparam name="TEntryPoint">The entry point class for the worker application.</typeparam>
public sealed class WorkerFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly XrmMockup365 xrm;
    private IReadOnlyList<WorkerRegistration>? workerRegistrations;

    public WireMockServer WireMockServer { get; }

    public IReadOnlyList<WorkerRegistration> WorkerRegistrations =>
        workerRegistrations ?? throw new InvalidOperationException(
            "WorkerRegistrations not yet available. Access Services first to initialize the factory.");

    public WorkerFactory(XrmMockup365 xrm, WireMockServer wireMockServer)
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
            // Capture WorkerRegistrations before modifying the service collection
            var registrations = services
                .Where(d => d.ServiceType == typeof(WorkerRegistration) && d.ImplementationInstance is not null)
                .Select(d => (WorkerRegistration)d.ImplementationInstance!)
                .ToList();
            workerRegistrations = registrations;

            // Replace Dataverse services with XrmMockup-backed implementations
            services.ReplaceDataverseServices(xrm);

            // Remove ALL QueueClient registrations (both regular and keyed services)
            var queueClientDescriptors = services
                .Where(d => d.ServiceType == typeof(QueueClient) || (d.IsKeyedService && d.KeyedImplementationType == typeof(QueueClient)))
                .ToList();
            foreach (var descriptor in queueClientDescriptors)
                services.Remove(descriptor);

            // Also remove keyed QueueClient services registered with factory pattern
            var keyedQueueClientDescriptors = services
                .Where(d => d.IsKeyedService && d.ServiceType == typeof(QueueClient))
                .ToList();
            foreach (var descriptor in keyedQueueClientDescriptors)
                services.Remove(descriptor);

            // Remove ALL IHostedService registrations to prevent background polling during tests
            // Tests use MessageExecutor to manually process queue messages
            var hostedServiceDescriptors = services
                .Where(d => d.ServiceType == typeof(IHostedService))
                .ToList();
            foreach (var descriptor in hostedServiceDescriptors)
                services.Remove(descriptor);
        });
    }
}
