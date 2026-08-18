using XrmBedrock.SharedContext;

namespace Tests;

/// <summary>
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
///
/// This file represents the Customer area of the solution.
/// </summary>
public partial class DataProducer
{
    internal Account ProduceValidAccount(Account? account) =>
        dao.Producer(account, e =>
        {
            e.EnsureValue(a => a.Name, "Just some example account");
            e.EnsureValue(a => a.EMailAddress1, "just@sampleaccount.com");
        });
}
