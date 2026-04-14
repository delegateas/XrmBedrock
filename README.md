# XrmBedrock
This project serves as a template for Dataverse projects. This project aims to make it easy to work with Dataverse and Azure together. This project shows that modern development paradigms are applicable to Dataverse.

This template will be updated. The current list is as follows
* Source control deployment
* New way of handling web resources.
* Deploying data.

# Initial setup
This project serves both as a template. For examples and demonstrations on how be used go to the examples branches. Generated files that are ignored in git are stored for your convenience in Setup/InitialSetup. It is safe to delete that folder and `copyInitialSetup.ps1`.

If you want to try it out right away, find an examples branch. Run `Setup/copyInitialSetup.ps1` using PowerShell. The script is not signed, so make sure to first run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. This copies the context for the demo and you should now be able to build and run all unit tests for the demo.

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
7. Verify the new command på open cmd and run "signtool /?" If not found restart computer and try again. 

---
# XRMBedrock Solution Template 
## Getting up and running
Follow these steps to setup your project correctly. After this you are ready to setup Azure DevOps.

## Rename file names and folders
- Rename file `XrmBedrock.slnx` => `ProjectName.slnx`
- Rename folder `src/Dataverse/Webresources/src/ctx_XrmBedrock` => `src/Dataverse/Webresources/src/prefix_SolutionName`

## Update values in WebResources files
- Update ``ctx_XrmBedrock`` to the new folder name in ``src/Dataverse/WebResources/esbuild.config.mjs``
- Update ``ctx_XrmBedrock`` to the new folder name in ``src/Dataverse/WebResources/package.json``
- Update ``ctx_XrmBedrock`` to the new folder name in ``src/Dataverse/WebResources/tsconfig.json``

## Generate new strong name key
Open the developer terminal in Visual Studio and write: 
`sn -k nameOfSolution.snk`

## Update values in Plugins.csproj
In the ``src/Dataverse/Plugins.csproj`` file, update the following:
- AssemblyName
- AssemblyOriginatorKeyFile
- Reference to the .snk file in the ``Exec`` element

## Update DAXIF Config
In the ``src\Tools\Daxif\_Config.fsx`` file, update/configure the following:
- Env
  - urls
  - The pipeline expects environment names Dev, Test, UAT and Prod - make sure that the names of the environment matches what the pipeline excepts, modify it if needed. 
- SolutionInfo
- PublisherInfo
- Change pluginDllName to match the new assembly name in ``src/Dataverse/Plugins.csproj``, with ILMerged prefix.  

Additional in tools folder; 
- ``src\Tools\Daxif\GenerateDataverseDomain.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\GenerateTypeScriptContext.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\username.txt``
  - add your username

## Ready for Dataverse
At this point your are ready for Dataverse development. The rest of the setup is regarding Azure setup and pipelines. The example pipeline assumes a full Azure and Dataverse setup.

## Generate a certificate
We generated a self-signed certificate to use with the Dataverse Managed Identity.

- Create a new password (at least 12 randomly generated characters is recommended). 
- Open an administrator powershell and run the ``Setup/generateNewCertificate.ps1`` file. 
- Use the following commands - remember to update "nameOfSolution" and "someRandomPassword":
  - `Set-ExecutionPolicy Bypass -Scope Process`
  - `./Setup/generateNewCertificate.ps1 -name "nameOfSolution" -friendlyName "nameOfSolution" -password "someRandomPassword" -environmentId "758cc81b-8df9-42cb-9d0a-a59482800d1f" -tenantId "12ec9b01-e104-4af3-b1f5-2ecfc065e1c2"`

Set the password in the signing part of the ``src/Dataverse/Plugins.csproj`` file in the ``Exec`` element.

## Update storage account environment variable
Create a new environment variable that will contain the storage account url

Update the reference to that variable in ``src\Dataverse\SharedPluginLogic\Logic\Azure\AzureConfigSetter.cs``

# Update files in .pipelines and Infrastructure
TODO: There should be a dedicated section for pipelines. Here it should be described what the default configurations do and what the prerequisites are.

Update values to match your solution:
- In ``.pipelines/Azure/Validate-DIF-Template`` update the resource group names
- In ``.pipelines/Infrastructure/main.bicep`` update the ``solutionId``  and ``companyId``

Tip: To locally validate your main.bicep, run the following commands:
``az login``
``az deployment group validate --resource-group <your-resource-group> --template-file main.bicep``
This will validate the template and show you any errors in the template (which the pipeline won't output)

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
i Server URL = The URL of the Dataverse environment (https://dev.crm4.dynamics.com)
ii Service Principal Id = The application (Client) id of the app registration
iii TenantI Id = Teant Id, can be found in Azure Portal
iV Service Connection Name = The name of the service connetion (e.g. Dataverse Dev)

Once created:
1.	Copy the Subject identifier as it is needed in the next step.

You now need to create a federated credential on your app registration.
1.	Open up the Azure Portal
2.	Find your app registration in the Entra ID
3.	Go to Manage > Certificates & secrets > Federated credentials > + Add credential
i For 'Federated credential scenario' select 'Other issuer'
ii Issuer = ADO SP Issuer 
iii	Type = ADO SP Explicit subject identifier
iv	Value = Paste the Subject identifier from the earlier step
v	Name = Name of your choice (e.g. PipelineDataverse)
4. Save

### How to create Azure Resource Manager service connections with federated credentials
1.	Go to Project Settings > Pipelines > Service connections > New service connection > Azure Resource Manager
2.	Identity type = App registration or managed identity (manual)
3.	Credential = Workload identity federation
4.	Service Connection Name = The name of the service connetion (e.g. Dev)
5.	Directory (tenant) Id = Teant Id, can be found in Azure Portal
6.	Click Next
7.	Copy the Issuer and Subject Identifier for later use
8.	Scope level = Subscription
9.	Subcription ID and Subscription Name can be found in the Azure Portal
10.	Application (client) ID = The client id of your app registration for the environment.

Here you are not able to verify and save, before the app registration in Azure portal have been modified. 
So keep as draft for now and head over to the azure portal 

You now need to create a federated credential on your app registration.
1.	Find your app registration in the Entra Id
2.	Go to Manage > Certificates & secrets > Federated credentials > + Add credential
3.	For 'Federated credential scenario' select 'Other issuer'
i.	Issuer = Paste the issuer (copied in earlier step)
ii.	Type = Explicit subject identifier
iii.	Value = Paste the subject (copied in earlier step)
iV.	Name = Name of your choice (e.g. Pipeline)
4. Add the app reg as a owner on the subscription or eventually on the resource group in the subscription

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
