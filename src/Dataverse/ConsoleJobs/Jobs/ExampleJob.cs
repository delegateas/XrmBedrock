using ConsoleJobs.Helpers;
using ConsoleJobs.Setup;
using Microsoft.Extensions.Logging;

namespace ConsoleJobs.Jobs;

#pragma warning disable CA1518 // It's an example, it's OK that its not referenced
#pragma warning disable CA1812 // It's an example, it's OK that its not referenced

internal sealed class ExampleJob : IJob
#pragma warning restore CA1812
#pragma warning restore CA1518
{
    private sealed class PrintableAccount
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
    }

    public void Run(EnvironmentConfig env)
    {
        var accounts = env.Dao.RetrieveList(xrm => xrm.AccountSet).Select(x => new PrintableAccount { Id = x.AccountId, Name = x.Name });

        env.Tracing.LogInformation("Account count: {Count}", accounts.Count());

        CsvHelper.WriteToCsv(env.CsvFolderPath, "ListOfAccounts.csv", accounts);
    }
}
