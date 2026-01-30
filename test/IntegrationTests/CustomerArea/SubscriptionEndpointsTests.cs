using System.Net;
using System.Net.Http.Json;
using DataverseService.CustomerArea.Dto;
using ExternalApi.CustomerArea.Requests;
using XrmBedrock.SharedContext;
using Task = System.Threading.Tasks.Task;

namespace IntegrationTests.CustomerArea;

public class SubscriptionEndpointsTests : TestBase
{
    public SubscriptionEndpointsTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetSubscriptionsForCustomer_WithValidCustomer_ReturnsOkWithSubscriptionsList()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(new Contact
        {
            FirstName = "Test",
            LastName = "Customer",
        });
        var product = Producer.ProduceValidProduct(new ctx_Product
        {
            ctx_Name = "Test Subscription",
        });
        var subscription = Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_Customer = customer.ToEntityReference(),
            ctx_Product = product.ToEntityReference(),
            ctx_StartDate = DateTime.UtcNow.AddMonths(-1),
        });

        // Act
        var response = await ApiClient.GetAsync(new Uri($"/api/subscriptions/{customer.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subscriptions = await response.Content.ReadFromJsonAsync<List<SubscriptionDto>>();
        subscriptions.Should().NotBeNull();
        subscriptions.Should().HaveCount(1);
        subscriptions![0].SubscriptionId.Should().Be(subscription.Id);
    }

    [Fact]
    public async Task GetSubscriptionsForCustomer_WithEmptyGuid_ReturnsBadRequest()
    {
        // Arrange
        var emptyCustomerId = Guid.Empty;

        // Act
        var response = await ApiClient.GetAsync(new Uri($"/api/subscriptions/{emptyCustomerId}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Be("\"Customer ID cannot be empty.\"");
    }

    [Fact]
    public async Task GetSubscriptionsForCustomer_CustomerWithNoSubscriptions_ReturnsOkWithEmptyList()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);

        // Act
        var response = await ApiClient.GetAsync(new Uri($"/api/subscriptions/{customer.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subscriptions = await response.Content.ReadFromJsonAsync<List<SubscriptionDto>>();
        subscriptions.Should().NotBeNull();
        subscriptions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubscriptionsForCustomer_CustomerWithMultipleSubscriptions_ReturnsAllSubscriptions()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);
        var product1 = Producer.ProduceValidProduct(new ctx_Product { ctx_Name = "Subscription A" });
        var product2 = Producer.ProduceValidProduct(new ctx_Product { ctx_Name = "Subscription B" });
        var product3 = Producer.ProduceValidProduct(new ctx_Product { ctx_Name = "Subscription C" });

        Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_Customer = customer.ToEntityReference(),
            ctx_Product = product1.ToEntityReference(),
        });
        Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_Customer = customer.ToEntityReference(),
            ctx_Product = product2.ToEntityReference(),
        });
        Producer.ProduceValidSubscription(new ctx_Subscription
        {
            ctx_Customer = customer.ToEntityReference(),
            ctx_Product = product3.ToEntityReference(),
        });

        // Act
        var response = await ApiClient.GetAsync(new Uri($"/api/subscriptions/{customer.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subscriptions = await response.Content.ReadFromJsonAsync<List<SubscriptionDto>>();
        subscriptions.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateSubscription_WithValidRequest_ReturnsCreatedWithSubscriptionId()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);
        var product = Producer.ProduceValidProduct(null);
        var startDate = DateTime.UtcNow.AddDays(7);
        var request = new CreateSubscriptionRequest
        {
            CustomerId = customer.Id,
            ProductId = product.Id,
            StartDate = startDate,
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/subscriptions/");

        var subscriptionId = await response.Content.ReadFromJsonAsync<Guid>();
        subscriptionId.Should().NotBeEmpty();

        // Verify the subscription was created
        var subscription = AdminDao.Retrieve<ctx_Subscription>(
            subscriptionId,
            s => s.ctx_Customer,
            s => s.ctx_Product);
        subscription.ctx_Customer.Id.Should().Be(customer.Id);
        subscription.ctx_Product.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task CreateSubscription_WithEmptyCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var product = Producer.ProduceValidProduct(null);
        var request = new CreateSubscriptionRequest
        {
            CustomerId = Guid.Empty,
            ProductId = product.Id,
            StartDate = DateTime.UtcNow,
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Be("\"Customer ID cannot be empty.\"");
    }

    [Fact]
    public async Task CreateSubscription_WithEmptyProductId_ReturnsBadRequest()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);
        var request = new CreateSubscriptionRequest
        {
            CustomerId = customer.Id,
            ProductId = Guid.Empty,
            StartDate = DateTime.UtcNow,
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Be("\"Product ID cannot be empty.\"");
    }

    [Fact]
    public async Task CreateSubscription_WithNullStartDate_UsesCurrentDateAsDefault()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);
        var product = Producer.ProduceValidProduct(null);
        var beforeRequest = DateTime.UtcNow;
        var request = new CreateSubscriptionRequest
        {
            CustomerId = customer.Id,
            ProductId = product.Id,
            StartDate = null,
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/subscriptions", request);
        var afterRequest = DateTime.UtcNow;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var subscriptionId = await response.Content.ReadFromJsonAsync<Guid>();

        var subscription = AdminDao.Retrieve<ctx_Subscription>(subscriptionId, s => s.ctx_StartDate);
        subscription.ctx_StartDate.Should().BeOnOrAfter(beforeRequest);
        subscription.ctx_StartDate.Should().BeOnOrBefore(afterRequest);
    }

    [Fact]
    public async Task CreateSubscription_WithFutureStartDate_SetsCorrectStartDate()
    {
        // Arrange
        var customer = Producer.ProduceValidContact(null);
        var product = Producer.ProduceValidProduct(null);
        var futureStartDate = DateTime.UtcNow.AddMonths(1);
        var request = new CreateSubscriptionRequest
        {
            CustomerId = customer.Id,
            ProductId = product.Id,
            StartDate = futureStartDate,
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/subscriptions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var subscriptionId = await response.Content.ReadFromJsonAsync<Guid>();

        var subscription = AdminDao.Retrieve<ctx_Subscription>(subscriptionId, s => s.ctx_StartDate);
        subscription.ctx_StartDate.Should().BeCloseTo(futureStartDate, TimeSpan.FromSeconds(1));
    }
}
