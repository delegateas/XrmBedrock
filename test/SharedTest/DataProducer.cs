using SharedContext.Dao;
using XrmBedrock.SharedContext;
using Task = XrmBedrock.SharedContext.Task;

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

    internal Account ProduceValidAccount(Account? account)
    {
        return adminDao.Producer(account, e =>
        {
            e.EnsureValue(a => a.Name, "Silkeborg sygehus");
            e.EnsureValue(a => a.EMailAddress1, "silkeborg@kommune.dk");
        });
    }

    internal Account ProduceValidRegionAccount(Account? account)
    {
        return adminDao.Producer(account, e =>
        {
            e.EnsureValue(p => p.Name, "Region Midt");
        });
    }

    internal Contact ProduceValidContact(Contact? person)
    {
        return adminDao.Producer(ConstructValidContact(person));
    }

    internal Contact ConstructValidContact(Contact? person)
    {
        return adminDao.Constructor(person, e =>
        {
            e.EnsureValue(c => c.FirstName, "Paul");
            e.EnsureValue(c => c.LastName, "Hansen");
            e.EnsureValue(c => c.EMailAddress1, "SomeEmail@SomeFakeDomain.test");
        });
    }

    internal static DateTime GetUTCDate(int year, int month, int day)
    {
        return new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    internal Task ConstructValidTask(Task? task)
    {
        return adminDao.Constructor(task, e =>
        {
            e.EnsureValue(t => t.Subject, "Some task");
        });
    }

    internal Template ProduceValidEmailTemplate(Template? queue)
    {
        return adminDao.Producer(queue, e =>
        {
            e.EnsureValue(c => c.Title, "Some title");
        });
    }

    internal Queue ProduceValidQueue(Queue? queue)
    {
        return adminDao.Producer(queue, e =>
        {
            e.EnsureValue(c => c.Name, "Some queue");
        });
    }

    internal mgs_Product ProduceValidProduct(mgs_Product? product)
    {
        return adminDao.Producer(product, e =>
        {
            e.EnsureValue(p => p.mgs_Name, "Some product");
            e.EnsureValue(p => p.mgs_Price, 100m);
        });
    }

    internal mgs_InvoiceCollection ProduceValidInvoiceCollection(mgs_InvoiceCollection? invoiceCollection) =>
        adminDao.Producer(invoiceCollection, e =>
        {
            e.EnsureValue(i => i.mgs_Name, "Some name");
            e.EnsureValue(i => i.mgs_InvoiceUntil, DateTime.UtcNow);
        });

    internal mgs_Subscription ProduceValidSubscription(mgs_Subscription? subscription) =>
        adminDao.Producer(subscription, e =>
        {
            e.EnsureValue(s => s.mgs_StartDate, DateTime.UtcNow);
            e.EnsureValue(s => s.mgs_Product, () => ProduceValidProduct(null).ToEntityReference());
            e.EnsureValue(s => s.mgs_Contact, () => ProduceValidContact(null).ToEntityReference());
        });
}