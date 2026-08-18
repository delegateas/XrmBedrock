using XrmBedrock.SharedContext;

namespace Tests;

/// <summary>
/// This is a SAMPLE file and you should remove it as soon as you have made your first real ProduceValidXXX method in a file like this representing one of the areas of your solution.
/// We generally try to avoid these sample files and folders in the solution and refer to the example brances in the repo but solution will not build without at least one reference to dao, so an exception is made in this case.
///
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
/// </summary>
public partial class DataProducer
{
    internal Contact SampleProduceValidContact(Contact? contact) =>
        dao.Producer(contact, e =>
        {
            e.EnsureValue(x => x.FirstName, $"John");
            e.EnsureValue(x => x.LastName, $"Doe");
        });
}
