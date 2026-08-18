using Task= XrmBedrock.SharedContext.Task;

namespace Tests;

/// <summary>
/// The use of partial classes allows us to split the DataProducer into multiple files, each representing a different area of the solution. This is important as the DataProducer can become quite large and unwieldy if all methods are in a single file.
///
/// This file represents the Activity area of the solution.
/// </summary>
public partial class DataProducer
{
    internal Task ProduceValidTask(Task? task) =>
       dao.Producer(task, e =>
       {
           e.EnsureValue(x => x.Subject, $"Test Task {GetUniqueNumber()}");
           e.EnsureValue(x => x.ScheduledStart, DateTime.Now);
           e.EnsureValue(x => x.ScheduledEnd, DateTime.Now.AddHours(1));
       });
}
