# ConsoleJobs
ConsoleJobs is a framework that streamlines the development of jobs/scripts that run against D365 using self-contained, single-configured jobs with CSV read/write capability.

## Features
- **Self-contained jobs.** You write the entirety of your ConsoleJob in one file.
- **Centralized configuration.** All necessary configuration is in App.config.
- **Methods for reading and writing to CSV files.** Generic read and write methods are provided with minimal per-job setup.

## Configuration
In App.config, define the following parameters:

`Environment` The environment to run your ConsoleJob against.

`JobClassName` The full name of the ConsoleJob to run.

`DevEnv`, `TestEnv`, `UatEnv`, `ProdEnv`: URLs of your Dev, Test, UAT and Prod environments.

`CsvSeparator` Separator to used for CSV files. Configured here for convenience, due to Excel's regional differences.

`AuthType` The authentication type for connecting to Dynamics 365 (set to "OAuth").

`Username` The email address of a user that has access to the CRM-environment.

`LoginPrompt` When to display the login prompt (set to "Always").

`AppId` The application ID for Azure AD authentication. Supplied by Microsoft: https://learn.microsoft.com/en-us/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters. No need to change this.

`RedirectUri` The redirect URI for OAuth authentication flow. Supplied by Microsoft: https://learn.microsoft.com/en-us/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#connection-string-parameters. No need to change this.

## Usage
1. Create your job as a class implementing the `IJob` interface in the /Jobs directory. 
2. Set the `ConsoleJobs` project as your startup project in Visual Studio.
3. In the App.config, make sure to configure `JobClassName`, `Environment`, `DevEnv`, `TestEnv`, `UatEnv`, `ProdEnv` and `Username`.
4. Build and run the `ConsoleJobs` project to run your job.

### Adding Additional Environments
To add additional environments, create an entry in App.config for it in the same fashion as the other environments, expand the `EnvironmentsEnum` and `GetUrlFromEnvironment()` in `EnvironmentConfig.cs` to include your new environment.