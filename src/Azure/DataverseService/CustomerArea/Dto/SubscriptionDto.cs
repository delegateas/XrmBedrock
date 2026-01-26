using XrmBedrock.SharedContext;

namespace DataverseService.CustomerArea.Dto;

public sealed record SubscriptionDto(
    Guid SubscriptionId,
    string? Name,
    Guid CustomerId,
    string? CustomerName,
    Guid ProductId,
    string? ProductName,
    DateTime? StartDate,
    DateTime? EndDate,
    ctx_SubscriptionState? State);
