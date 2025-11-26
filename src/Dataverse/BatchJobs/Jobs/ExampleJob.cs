#:package DataverseConnection@1.1.1
#:package Azure.Identity@1.13.2
#:project ../BatchJobs.csproj

using BatchJobs;

var ctx = JobSetup.Initialize(args);

var accounts = ctx.Dao.RetrieveList(xrm => xrm.AccountSet)
    .Select(a => new { a.AccountId, a.Name })
    .ToList();

Console.WriteLine($"Found {accounts.Count} accounts");
