using DataverseService.Workers;
using EconomyAreaWorker.Workers;
using SharedDomain;
using SharedDomain.EconomyArea;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace IntegrationTests.Infrastructure;

/// <summary>
/// Tests verifying the auto-discovery worker infrastructure works correctly.
/// These tests validate WorkerRegistration discovery, message routing, and end-to-end queue processing.
/// </summary>
public class WorkerInfrastructureTests : TestBase
{
    private readonly WorkerFactory<CreateInvoicesWorker> workerFactory;

    public WorkerInfrastructureTests(XrmMockupFixture fixture)
        : base(fixture)
    {
        // Create a dedicated WorkerFactory for these tests to inspect registrations
        workerFactory = new WorkerFactory<CreateInvoicesWorker>(Xrm, Server);

        // Initialize the factory to populate WorkerRegistrations
        _ = workerFactory.Services;
    }

    [Fact]
    public void WorkerFactory_DiscoversWorkerRegistrations()
    {
        // Arrange & Act - WorkerRegistrations are populated during factory initialization

        // Assert - Verify that WorkerRegistrations contains at least one registration
        workerFactory.WorkerRegistrations.Should().NotBeEmpty(
            "WorkerFactory should auto-discover WorkerRegistrations from the service collection");

        // Verify that the CreateInvoicesWorker registration is present
        var createInvoicesRegistration = workerFactory.WorkerRegistrations
            .FirstOrDefault(r => r.QueueName == QueueNames.CreateInvoicesQueue);

        createInvoicesRegistration.Should().NotBeNull(
            "WorkerFactory should discover the CreateInvoicesWorker registration");
    }

    [Fact]
    public void WorkerRegistration_HasCorrectMetadata_ForCreateInvoicesWorker()
    {
        // Arrange - Get the registration for CreateInvoicesWorker

        // Act
        var registration = workerFactory.WorkerRegistrations
            .FirstOrDefault(r => r.WorkerType == typeof(CreateInvoicesWorker));

        // Assert - Verify registration exists
        registration.Should().NotBeNull("CreateInvoicesWorker should be registered");

        // Verify the registration has the correct values derived via reflection
        registration!.QueueName.Should().Be(
            QueueNames.CreateInvoicesQueue,
            "Registration QueueName should match the queue constant from [FromKeyedServices] attribute");
        registration.MessageType.Should().Be<CreateInvoicesMessage>(
            "Registration MessageType should match the generic parameter of QueueWorker<TMessage>");
        registration.WorkerType.Should().Be<CreateInvoicesWorker>(
            "Registration WorkerType should be the worker class");
    }

    [Fact]
    public void WorkerRegistration_HasCorrectStructure()
    {
        // Arrange & Act
        var registrations = workerFactory.WorkerRegistrations;

        // Assert - Verify each registration has all required properties set
        foreach (var registration in registrations)
        {
            registration.WorkerType.Should().NotBeNull(
                "Each registration should have a WorkerType");
            registration.MessageType.Should().NotBeNull(
                "Each registration should have a MessageType");
            registration.QueueName.Should().NotBeNullOrWhiteSpace(
                "Each registration should have a valid QueueName");

            // Verify the worker type inherits from QueueWorker<TMessage>
            var baseType = registration.WorkerType.BaseType;
            var inheritsFromQueueWorker = false;
            while (baseType is not null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(QueueWorker<>))
                {
                    inheritsFromQueueWorker = true;
                    break;
                }

                baseType = baseType.BaseType;
            }

            inheritsFromQueueWorker.Should().BeTrue(
                $"Worker type {registration.WorkerType.Name} should inherit from QueueWorker<TMessage>");
        }
    }

    [Fact]
    public async Task MessageExecutor_RoutesMessageByQueueName_ToCorrectService()
    {
        // Arrange - Create test data required for invoice creation
        _ = Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_StartDate = DateTime.Now.AddMonths(-1),
            ctx_Product = Producer.ProduceValidProduct(new ctx_Product
            {
                ctx_BillingInterval = ctx_billinginterval.Monthly,
            }).ToEntityReference(),
        });

        var invoiceCollection = Producer.ProduceValidInvoiceCollection(null);

        // Act - Trigger the plugin that sends a message to the Azure queue
        AdminDao.Update(new ctx_InvoiceCollection(invoiceCollection.Id)
        {
            statuscode = ctx_InvoiceCollection_statuscode.CreateInvoices,
        });

        // Process the queue messages through MessageExecutor
        await MessageExecutor.SendMessages();

        // Assert - Verify the message was routed correctly by checking the data changes
        var retrievedInvoiceCollection = AdminDao.Retrieve<ctx_InvoiceCollection>(
            invoiceCollection.Id,
            x => x.statuscode);

        retrievedInvoiceCollection.statuscode.Should().Be(
            ctx_InvoiceCollection_statuscode.InvoicesCreated,
            "Invoice collection status should be updated after message processing");

        // Verify that transactions were created (proving the service was invoked)
        var transactions = AdminDao.RetrieveList(xrm => xrm.ctx_TransactionSet);
        transactions.Should().NotBeEmpty(
            "Transactions should be created when the message is processed by the correct service");
    }

    [Fact]
    public Task MessageExecutor_ThrowsForUnknownQueue()
    {
        // Arrange - Store a message with an unknown queue name
        MessageExecutor.StoreMessage(new AwaitingMessage(
            "unknownqueue",
            "PFF8RW1wdHlNZXNzYWdlPjwvRW1wdHlNZXNzYWdlPg=="));

        // Act & Assert - Verify that SendMessages throws InvalidOperationException
        var act = () => MessageExecutor.SendMessages();

        return act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*unknownqueue*");
    }

    [Fact]
    public void WorkerRegistrations_AreAccessibleThroughWorkerFactory()
    {
        // Arrange - Factory is already initialized in constructor

        // Act
        var registrations = workerFactory.WorkerRegistrations;

        // Assert - Verify registrations are accessible and properly typed
        registrations.Should().BeAssignableTo<IReadOnlyList<WorkerRegistration>>(
            "WorkerRegistrations should be IReadOnlyList<WorkerRegistration>");
        registrations.Should().AllBeOfType<WorkerRegistration>(
            "All items should be WorkerRegistration instances");
    }

    [Fact]
    public void WorkerFactory_ThrowsWhenAccessingRegistrationsBeforeServicesInitialized()
    {
        // Arrange - Create a new factory without initializing Services
        using var uninitializedFactory = new WorkerFactory<CreateInvoicesWorker>(Xrm, Server);

        // Act & Assert - Accessing WorkerRegistrations before Services should throw
        var act = () => uninitializedFactory.WorkerRegistrations;

        act.Should().Throw<InvalidOperationException>().WithMessage("*Services*");
    }

    [Fact]
    public void AllRegisteredQueues_AreInQueueNamesAllQueuesCollection()
    {
        // Arrange
        var registrations = workerFactory.WorkerRegistrations;

        // Act & Assert - Verify each registered queue name is in QueueNames.AllQueues
        foreach (var registration in registrations)
        {
            var queueName = registration.QueueName;
            var workerName = registration.WorkerType.Name;
            QueueNames.AllQueues.Should().Contain(
                queueName,
                $"Queue '{queueName}' from worker {workerName} should be defined in QueueNames.AllQueues");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            workerFactory.Dispose();
        }

        base.Dispose(disposing);
    }
}
