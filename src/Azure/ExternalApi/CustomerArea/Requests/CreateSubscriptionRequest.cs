namespace ExternalApi.CustomerArea.Requests;

public sealed record CreateSubscriptionRequest
{
    public Guid CustomerId { get; init; }

    public Guid ProductId { get; init; }

    public DateTime? StartDate { get; init; }
}
