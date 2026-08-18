using XrmBedrock.SharedContext;

namespace Tests;

/// <summary>
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
///
/// This one represents Utility which is not really an area of the functional domain but handy parts of the platform.
/// </summary>
public partial class DataProducer
{
    internal DuplicateRule ProduceValidDuplicateRule(DuplicateRule? duplicateRule) =>
        elevatedDao.Producer(duplicateRule, e =>
        {
            e.EnsureValue(x => x.Name, $"Test Duplicate Rule {GetUniqueNumber()}");
        });
}
