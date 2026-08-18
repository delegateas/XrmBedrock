using XrmBedrock.SharedContext;

namespace Tests;

/// <summary>
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
///
/// This file represents the Customer area of the solution.
/// </summary>
public partial class DataProducer
{
    internal Contact ProduceValidContact(Contact? person) =>
        dao.Producer(person, e =>
        {
            e.EnsureValue(c => c.FirstName, "Paul");
            e.EnsureValue(c => c.LastName, "Hansen");
            e.EnsureValue(c => c.EMailAddress1, "SomeEmail@SomeFakeDomain.test");
        });
}
