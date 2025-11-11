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

    internal DuplicateRule ProduceValidDuplicateRule(DuplicateRule? duplicateRule) =>
        adminDao.Producer(duplicateRule, e =>
        {
            e.EnsureValue(x => x.Name, $"Test Duplicate Rule {GetUniqueNumber()}");
        });

    internal Task ProduceValidTask(Task? task) =>
        adminDao.Producer(task, e =>
        {
            e.EnsureValue(x => x.Subject, $"Test Task {GetUniqueNumber()}");
            e.EnsureValue(x => x.ScheduledStart, DateTime.Now);
            e.EnsureValue(x => x.ScheduledEnd, DateTime.Now.AddHours(1));
        });

    internal Account ProduceValidAccount(Account? account) =>
        adminDao.Producer(account, e =>
        {
            e.EnsureValue(a => a.Name, "Just some example account");
            e.EnsureValue(a => a.EMailAddress1, "just@sampleaccount.com");
        });
}