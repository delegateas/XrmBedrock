using SharedContext.Dao;

namespace SharedTest;

/// <summary>
/// Helper for producing data for tests.
/// </summary>
public partial class DataProducer
{
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IDataverseAccessObject adminDao;
#pragma warning restore S4487 // Unread "private" fields should be removed

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
}