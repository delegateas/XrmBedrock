using XrmBedrock.SharedContext;

namespace Tests;

/// <summary>
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
///
/// This file represents the Customer area of the solution.
/// </summary>
public partial class DataProducer
{
    internal ctx_Product ProduceValidProduct(ctx_Product? product) =>
        elevatedDao.Producer(product, e =>
        {
            e.EnsureValue(p => p.ctx_Name, "Some product");
            e.EnsureValue(p => p.ctx_Price, 100m);
        });

    internal ctx_InvoiceCollection ProduceValidInvoiceCollection(ctx_InvoiceCollection? invoiceCollection) =>
        dao.Producer(invoiceCollection, e =>
        {
            e.EnsureValue(i => i.ctx_Name, "Some name");
            e.EnsureValue(i => i.ctx_InvoiceUntil, DateTime.UtcNow);
        });

    internal ctx_Subscription ProduceValidSubscription(ctx_Subscription? subscription) =>
        dao.Producer(subscription, e =>
        {
            e.EnsureValue(s => s.ctx_StartDate, DateTime.UtcNow);
            e.EnsureValue(s => s.ctx_Product, () => ProduceValidProduct(null).ToEntityReference());
            e.EnsureValue(s => s.ctx_Customer, () => ProduceValidContact(null).ToEntityReference());
        });
}
