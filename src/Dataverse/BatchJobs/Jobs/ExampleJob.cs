#:package DataverseConnection@1.1.1
#:package Azure.Identity@1.13.2
#:project ../BatchJobs.csproj

using Azure.Identity;
using DataverseConnection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

// === CONFIGURATION ===
var dataverseUrl = args.Length > 0 ? args[0] : "https://YOUR-ENV.crm4.dynamics.com";

// === SETUP ===
var credential = new DefaultAzureCredential();
var services = new ServiceCollection();
services.AddDataverseWithOrganizationServices(options =>
{
    options.TokenCredential = credential;
    options.DataverseUrl = dataverseUrl;
});
var sp = services.BuildServiceProvider();
var orgService = sp.GetRequiredService<IOrganizationService>();
var dao = new DataverseAccessObject(orgService);

// === JOB LOGIC ===
Console.WriteLine($"Connected to {dataverseUrl}");

var accounts = dao.RetrieveList(xrm => xrm.AccountSet)
    .Select(a => new { a.AccountId, a.Name })
    .ToList();

Console.WriteLine($"Found {accounts.Count} accounts");
