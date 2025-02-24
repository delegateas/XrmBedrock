namespace SharedDomain.EconomyArea;

public record CreateTransactionsRequest(Guid SubscriptionId, DateTime End);