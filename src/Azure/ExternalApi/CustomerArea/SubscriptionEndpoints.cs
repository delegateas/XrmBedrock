using DataverseService.CustomerArea;
using DataverseService.CustomerArea.Dto;
using ExternalApi.CustomerArea.Requests;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExternalApi.CustomerArea;

public static class SubscriptionEndpoints
{
    private const string RoutesPrefix = "/api/subscriptions";

    public static void MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutesPrefix);

        group.MapGet("/{customerId:guid}", GetSubscriptionsForCustomer)
            .WithName("GetSubscriptionsForCustomer")
            .WithSummary("Get subscriptions for a customer")
            .WithDescription("Retrieves all subscriptions associated with a specific customer. Returns subscription details including product information, dates, and status.");

        group.MapPost("/", CreateSubscription)
            .WithName("CreateSubscription")
            .WithSummary("Create a new subscription")
            .WithDescription("Creates a new subscription linking a customer to a product. Requires the customer ID, product ID, and start date. Only available to interactive users.")
            .RequireAuthorization("InteractiveUsers");

        group.MapGet("/admin/{customerId:guid}", GetSubscriptionsForCustomerAdmin)
            .WithName("GetSubscriptionsForCustomerAdmin")
            .WithSummary("Get subscriptions for a customer (Admin)")
            .WithDescription("Admin endpoint to retrieve all subscriptions for a customer. Only accessible via client credentials (service-to-service).")
            .RequireAuthorization("ClientCredentialsOnly");
    }

    public static async Task<Results<Ok<List<SubscriptionDto>>, BadRequest<string>>> GetSubscriptionsForCustomer(
        Guid customerId,
        DataverseSubscriptionService service)
    {
        ArgumentNullException.ThrowIfNull(service);

        var result = await service.GetSubscriptionsForCustomer(customerId);

        return result.Match<Results<Ok<List<SubscriptionDto>>, BadRequest<string>>>(
            subscriptions => TypedResults.Ok(subscriptions),
            validationError => TypedResults.BadRequest(validationError.Message));
    }

    public static async Task<Results<Created<Guid>, BadRequest<string>>> CreateSubscription(
        CreateSubscriptionRequest request,
        DataverseSubscriptionService service)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);

        var startDate = request.StartDate ?? DateTime.UtcNow;

        var result = await service.CreateSubscription(
            request.CustomerId,
            request.ProductId,
            startDate);

        return result.Match<Results<Created<Guid>, BadRequest<string>>>(
            subscriptionId => TypedResults.Created($"{RoutesPrefix}/{subscriptionId}", subscriptionId),
            validationError => TypedResults.BadRequest(validationError.Message));
    }

    public static Task<Results<Ok<List<SubscriptionDto>>, BadRequest<string>>> GetSubscriptionsForCustomerAdmin(
        Guid customerId,
        DataverseSubscriptionService service)
    {
        return GetSubscriptionsForCustomer(customerId, service);
    }
}
