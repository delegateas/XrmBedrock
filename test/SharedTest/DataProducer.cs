using SharedContext.Dao;
using XrmBedrock.SharedContext;

namespace SharedTest;

public class DataProducer
{
    private readonly IDataverseAccessObject adminDao;

    public DataProducer(IDataverseAccessObject adminDao)
    {
        this.adminDao = adminDao;
    }

    private readonly Random random = new Random((int)DateTime.Now.Ticks);
    private readonly Dictionary<int, bool> used = new Dictionary<int, bool>();

    internal int GetUniqueNumber()
    {
#pragma warning disable SCS0005
#pragma warning disable CA5394
        var next = random.Next();
#pragma warning restore SCS0005
#pragma warning restore CA5394

        try
        {
            used.Add(next, true);
            return next;
        }
        catch (ArgumentException)
        {
            return GetUniqueNumber();
        }
    }

    internal DuplicateRule ProduceValidDuplicateRule(DuplicateRule? duplicateRule) =>
        adminDao.Producer(duplicateRule, e =>
        {
            e.EnsureValue(x => x.Name, $"Test Duplicate Rule {GetUniqueNumber()}");
        });

    internal Contact ProduceValidContact(Contact? person) =>
        adminDao.Producer(person, e =>
        {
            e.EnsureValue(c => c.FirstName, "Paul");
            e.EnsureValue(c => c.LastName, "Hansen");
            e.EnsureValue(c => c.EMailAddress1, "SomeEmail@SomeFakeDomain.test");
        });

    internal ctx_Product ProduceValidProduct(ctx_Product? product) =>
        adminDao.Producer(product, e =>
        {
            e.EnsureValue(p => p.ctx_Name, "Some product");
            e.EnsureValue(p => p.ctx_Price, 100m);
        });

    internal ctx_InvoiceCollection ProduceValidInvoiceCollection(ctx_InvoiceCollection? invoiceCollection) =>
        adminDao.Producer(invoiceCollection, e =>
        {
            e.EnsureValue(i => i.ctx_Name, "Some name");
            e.EnsureValue(i => i.ctx_InvoiceUntil, DateTime.UtcNow);
        });

    internal ctx_Subscription ProduceValidSubscription(ctx_Subscription? subscription) =>
        adminDao.Producer(subscription, e =>
        {
            e.EnsureValue(s => s.ctx_StartDate, DateTime.UtcNow);
            e.EnsureValue(s => s.ctx_Product, () => ProduceValidProduct(null).ToEntityReference());
            e.EnsureValue(s => s.ctx_Customer, () => ProduceValidContact(null).ToEntityReference());
        });
}
