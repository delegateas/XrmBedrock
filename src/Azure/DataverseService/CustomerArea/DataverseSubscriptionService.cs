using DataverseService.CustomerArea.Dto;
using DataverseService.CustomerArea.Errors;
using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using OneOf;
using XrmBedrock.SharedContext;

namespace DataverseService.CustomerArea;

public sealed class DataverseSubscriptionService(IDataverseAccessObjectAsync dao)
{
    public async Task<OneOf<List<SubscriptionDto>, ValidationError>> GetSubscriptionsForCustomer(Guid customerId)
    {
        if (customerId == Guid.Empty)
            return new ValidationError("Customer ID cannot be empty.");

        var subscriptions = await dao.RetrieveListAsync(xrm =>
            xrm.ctx_SubscriptionSet
                .Where(s => s.ctx_Customer != null && s.ctx_Customer.Id == customerId)
                .Select(s => new
                {
                    s.Id,
                    s.ctx_Name,
                    s.ctx_Customer,
                    s.ctx_Product,
                    s.ctx_StartDate,
                    s.ctx_EndDate,
                    s.statecode,
                }));

        var productIds = subscriptions
            .Where(s => s.ctx_Product != null)
            .Select(s => s.ctx_Product!.Id)
            .Distinct()
            .ToArray();

        var products = productIds.Length > 0
            ? await dao.RetrieveMultipleByIdsAsync<ctx_Product>(productIds, p => p.ctx_Name)
            : [];

        var productLookup = products.ToDictionary(p => p.Id, p => p.ctx_Name);

        var subscriptionDtos = subscriptions.Select(s => new SubscriptionDto(
            s.Id,
            s.ctx_Name,
            customerId,
            s.ctx_Customer?.Name,
            s.ctx_Product?.Id ?? Guid.Empty,
            s.ctx_Product != null && productLookup.TryGetValue(s.ctx_Product.Id, out var productName) ? productName : null,
            s.ctx_StartDate,
            s.ctx_EndDate,
            s.statecode)).ToList();

        return subscriptionDtos;
    }

    public async Task<OneOf<Guid, ValidationError>> CreateSubscription(Guid customerId, Guid productId, DateTime startDate)
    {
        if (customerId == Guid.Empty)
            return new ValidationError("Customer ID cannot be empty.");

        if (productId == Guid.Empty)
            return new ValidationError("Product ID cannot be empty.");

        var subscription = new ctx_Subscription
        {
            ctx_Customer = new EntityReference(Contact.EntityLogicalName, customerId),
            ctx_Product = new EntityReference(ctx_Product.EntityLogicalName, productId),
            ctx_StartDate = startDate,
        };

        var subscriptionId = await dao.CreateAsync(subscription);

        return subscriptionId;
    }
}
