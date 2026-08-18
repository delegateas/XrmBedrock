using SharedContext.Dao;

namespace Tests;

/// <summary>
/// The <see cref="DataProducer"/> is used to produce data for unit tests. It is important to use these instead of just creating entities directly, as the producer will ensure that the data is valid and that any required related entities are created as well.
/// A new plugin imposing restrictions on the creation of an instance of an entity will when using <see cref="DataProducer"/> only require changes to ProduceValidXXX methods and not changes in a range of unit tests that create entities directly.
/// Most uses are with dao as an admin dao, but there are situations where a user-context must be tested and a userDao will be suppplied. In these cases an adminDao should be supplied as elevatedDao to allow creation of related entities that the user-context does not have permission to create.
/// </summary>
/// <param name="dao">User or admin dao</param>
/// <param name="elevatedDao">Optional admin dao to allow creation of related entities</param>
public partial class DataProducer(IDataverseAccessObject dao, IDataverseAccessObject? elevatedDao = null)
{
    private readonly IDataverseAccessObject dao = dao;
    private readonly IDataverseAccessObject elevatedDao = elevatedDao ?? dao;

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
