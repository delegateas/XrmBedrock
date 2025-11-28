using System.Reflection;
using ConsoleJobs;

static string GetJobName(Type t)
{
    var name = t.Name;
    return name.EndsWith("Job", StringComparison.Ordinal) ? name[..^3] : name;
}

var jobs = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .ToDictionary(GetJobName, t => t, StringComparer.OrdinalIgnoreCase);

if (args.Length == 0 || !jobs.TryGetValue(args[0], out var jobType))
{
    Console.WriteLine("Usage: ConsoleJobs <jobname> [dataverse-url]");
    Console.WriteLine();
    Console.WriteLine("Available jobs:");
    foreach (var name in jobs.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
    {
        Console.WriteLine($"  {name}");
    }

    return 1;
}

var job = (IJob)Activator.CreateInstance(jobType)!;

var jobArgs = args.Skip(1).ToArray();
var ctx = JobSetup.Initialize(jobArgs);

job.Run(ctx);

return 0;
