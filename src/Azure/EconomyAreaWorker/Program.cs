using Azure.Identity;
using DataverseService;
using DataverseService.EconomyArea;
using DataverseService.Workers;
using EconomyAreaWorker.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataverse(
    builder.Environment.IsDevelopment()
        ? new AzureCliCredential()
        : new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")));

builder.Services.AddEconomyServices();

builder.Services.AddAreaWorkers("AZURE_STORAGE_QUEUE_URI", workers =>
{
    workers.AddWorker<CreateInvoicesWorker>();
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", () => Results.Ok("EconomyAreaWorker is running"));

app.MapHealthChecks("/health");

app.Run();
