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

    internal Account ProduceValidAccount(Account? account) => adminDao.Producer(ConstructValidAccount(account));

    internal Account ConstructValidAccount(Account? account)
    {
        return adminDao.Constructor(account, e =>
        {
            e.EnsureValue(a => a.Name, "Kattens værn");
            e.EnsureValue(a => a.EMailAddress1, "værn@kattenettet.dk");
        });
    }

    internal Contact ProduceValidContact(Contact? person) => adminDao.Producer(ConstructValidContact(person));

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
}