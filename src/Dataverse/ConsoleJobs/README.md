# ConsoleJobs

Ad-hoc console jobs for Dataverse operations.

## Usage

```bash
# List available jobs
dotnet run --project src/Dataverse/ConsoleJobs/ConsoleJobs.csproj

# Run a job with specific Dataverse URL
dotnet run --project src/Dataverse/ConsoleJobs/ConsoleJobs.csproj -- SolutionComponent https://your-env.crm4.dynamics.com
```

## Authentication

Uses `AzureCliCredential` - authenticate via Azure CLI before running:

```bash
az login
```

## Adding a new job

1. Create a new class in the `Jobs` folder
2. Implement `IJob` interface
3. Name the class with a `Job` suffix (e.g., `MyCustomJob`)

The job will be auto-discovered and available as `MyCustom`.

```csharp
using Microsoft.Extensions.Logging;

namespace ConsoleJobs.Jobs;

public class MyCustomJob : IJob
{
    public void Run(JobContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        ctx.Logger.LogInformation("Running my custom job...");
        // Use ctx.OrgService or ctx.Dao for Dataverse operations
    }
}
```
