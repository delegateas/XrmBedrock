# XrmBedrock
This project serves as a template for Dataverse projects. This project aims to make it easy to work with Dataverse and Azure together. This project shows that modern development paradigms are applicable to Dataverse.

This template will be updated. The current list is as follows
* Source control deployment
* New way of handling web resources.
* Deploying data.

---
## Prerequisites
Signtool is needed to sign the plugin assembly. It is included in the Windows SDK, which can be downloaded here: https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk/. 

If Signtool already installed, proceceed too next section. 
if not; Add signtool to system variables:
1. Press Win + R, type sysdm.cpl, hit Enter.
2. Go to Advanced → Environment Variables.
3. Under System variables, find Path, click Edit.
4. Click New, paste the folder path (the one containing signtool.exe).
Example:
  C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool
6. Click OK on all dialogs.
7. Verify the new command på open cmd and run "signtool /?" If not found restart computer and try again

Additionally install [PowerShell Core](https://github.com/PowerShell/PowerShell) (`pwsh`)

# Quick start (dotnet new)

1. Install the template from the repository root:

   ```bash
   dotnet new install .
   ```

2. Create a new project, replacing each placeholder value with your own:

   ```bash
   dotnet new xrmbedrock -n MyProject \
     --company-name MyOrg \
     --publisher-prefix abc \
     --solution-id mysol \
     --dev-url https://myorg-dev.crm4.dynamics.com \
     --test-url https://myorg-test.crm4.dynamics.com \
     --uat-url https://myorg-uat.crm4.dynamics.com \
     --prod-url https://myorg-prod.crm4.dynamics.com \
     --rg-name myorg-mysol \
     --cert-password MySecurePassword123 \
     --username user@myorg.onmicrosoft.com
   ```

3. Post-setup runs automatically (generates a strong name key, plugin signing certificate, restores tools, installs npm packages, and generates Dataverse context files). 
   You will be prompted to authenticate with your Dataverse environment via a browser popup. Requires [PowerShell Core](https://github.com/PowerShell/PowerShell) (`pwsh`).

4. Once post-setup completes, initialize git and create the initial commit:

   ```bash
   git init && git add -A && git commit -m "Initial project setup from XrmBedrock template"
   ```

To uninstall the template: `dotnet new uninstall .`

# Regenerating Dataverse Context

The Dataverse context files (C# proxies, TypeScript typings, and test metadata) are generated from your Dataverse environment during post-template setup using the F# scripts in `src/Tools/Daxif/`. You can regenerate them at any time:

```bash
dotnet fsi src/Tools/Daxif/GenerateCSharpContext.fsx
dotnet fsi src/Tools/Daxif/GenerateTypeScriptContext.fsx
```

# Azure Setup

## Federated Credentials

To configure federated credentials for the Dataverse Managed Identity, run the following script. It loads the generated `plugincert.pfx` and prints the issuer, subject, thumbprint, and hash needed for Azure AD configuration:

```bash
pwsh Setup/printFederatedCredentials.ps1 -password "<your-cert-password>" -environmentId "<environment-guid>" -tenantId "<tenant-guid>"
```

## Storage account environment variable

Create a new environment variable that will contain the storage account URL.

Update the reference to that variable in `src/Dataverse/SharedPluginLogic/Logic/Azure/AzureConfigSetter.cs`.

## Infrastructure validation

To locally validate your `main.bicep`, run:

```bash
az login
az deployment group validate --resource-group <your-resource-group> --template-file main.bicep
```

Note: When using `dotnet new`, `solutionId` and `companyId` in `Infrastructure/main.bicep` are set automatically via template parameters.

# Azure DevOps

## Environment
Under Pipelines > Environment, create an environment per Dataverse environment.
Note: The pipeline template uses Dev, Test, UAT, Prod.
Use these to control approvals of deployments, regarding approval gates etc.

## Library
Under Pipelines > Library, create a variable group per environment.
Note: The pipeline template uses Dev, Test, UAT, Prod.
The template assumes the following variables exist.
* ResourceGroupName
* DataverseUrl
* DataverseAppId (used by pipeline)
* DataverseSecret (used by pipeline)
* AzureClientId
* AzureClientSecret (only used for DAXIF)
* AzureTenantId (Needed for the managed identity record)
* AzureClientEAObjectId (Object id of the Enterprise Application related to the App registration)

## Service Connection
Under Project Settings > Pipelines > Service connections, create 2 service connections per azure environment of types Power Platform and Azure Resource Manager.
A service connection is used to authorize the pipeline against other services. The goal is to avoid secrets in the pipeline. Use the recommended settings with Workload Federated Credentials.
Note: The pipeline template uses Dev, Test, UAT, Prod.

###  How to create Power Platform service connections with federated credentials
1.	Go to Project Settings > Pipelines > Service connections > New service connection > Power Platform
2.	Select Workload Identity federation
3.	Fill in the form 
4. Server URL = The URL of the Dataverse environment (https://dev.crm4.dynamics.com)
5. Service Principal Id = The application (Client) id of the app registration
6. TenantI Id = Teant Id, can be found in Azure Portal
7. Service Connection Name = The name of the service connetion (e.g. Dataverse Dev)

Once created: Copy the Subject identifier and issuer as it is needed in the next step.

You now need to create a federated credential on your app registration.
1. Find your app registration in the Azure Portal
2. Go to Manage > Certificates & secrets > Federated credentials > + Add credential
3. For 'Federated credential scenario' select 'Other issuer'
4. Issuer = Paste the 'issuer' from the earlier step
5. Type = Explicit subject identifier
6. Value = Paste the 'Subject identifier' from the earlier step
7. Name = Name of your choice (e.g. PipelineDataverse)

### How to create Azure Resource Manager service connections with federated credentials
1. Go to Project Settings > Pipelines > Service connections > New service connection > Azure Resource Manager
2. Identity type = App registration or managed identity (manual)
3. Credential = Workload identity federation
4. Service Connection Name = The name of the service connection (e.g. Dev)
5. Directory (tenant) Id = Tenant Id, can be found in Azure Portal
6. Click Next
7. Copy the Issuer and Subject Identifier for later use
8. Scope level = Subscription
9. Subscription ID and Subscription Name can be found in the Azure Portal
10. Application (client) ID = The client id of your app registration for the environment.

You now need to create a federated credential on your app registration.
1.	Find your app registration in the Entra Id
2.	Go to Manage > Certificates & secrets > Federated credentials > + Add credential
3.	For 'Federated credential scenario' select 'Other issuer'
4.	Issuer = Paste the issuer (copied in earlier step)
5.	Type = Explicit subject identifier
6.	Value = Paste the subject (copied in earlier step)
7.	Name = Name of your choice (e.g. Pipeline)

8. Add the app reg as a owner on the subscription or eventually on the resource group in the subscription

Then head back to ADO and verify and save the service connection.

## App registration privileges
Remember to give your app reg permission to assign roles.
The easiest method is to add it as owner to the subscription and restrict the role assignment to the ones bicep assigns.
The template uses Storage Queue Data Contributor

## Managed identity
A managed identity is created by the bicep deploy. This is what Azure uses to call back into Dataverse. Make sure it is created as an app user. Search for the client id of the managed identity, you will not find it by name.

## Pipeline and PR validation
Remember to "uncomment" the `build.yaml` workflow trigger, for use of build validation in pull requests.

## TODO
* Improve validation of infrastructure to be easier to manage
* Auto set storage account environment variable
* Auto user creation for managed identity
