# XrmBedrock
This project serves as a template for Dataverse projects. This project aims to make it easy to work with Dataverse and Azure together. This project shows that modern development paradigms are applicable to Dataverse.

This template will be updated. The current list is as follows
* Source control deployment
* New way of handling web resources.
* Deploying data.

# Initial setup
This project serves both as a template. For examples and demonstrations on how be used go to the examples branches. Generated files that are ignored in git are stored for your convenience in Setup/InitialSetup. It is safe to delete that folder and `copyInitialSetup.ps1`.

If you want to try it out right away, find an examples branch. Run `Setup/copyInitialSetup.ps1` using PowerShell. The script is not signed, so make sure to first run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. This copies the context for the demo and you should now be able to build and run all unit tests for the demo.

# Getting up and running
Follow these steps to setup your project correctly. After this you are ready to setup Azure DevOps.

# Rename file names and folders
- Rename file `XrmBedrock.slnx` => `ProjectName.slnx`
- Rename folder `src/Dataverse/Webresources/src/ctx_XrmBedrock` => `src/Dataverse/Webresources/src/prefix_SolutionName`

# Update values in WebResources files
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
- ``src\Tools\Daxif\GenerateDataverseDomain.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\GenerateTypeScriptContext.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\username.txt``
  - add your username

## Generate a certificate
We generated a self-signed certificate to use with the Dataverse Managed Identity.

- Create a new password (at least 12 randomly generated characters is recommended). 
- Open an administrator powershell and run the ``Setup/generateNewCertificate.ps1`` file. 
- Use the following commands - remember to update "nameOfSolution" and "someRandomPassword":
  - `Set-ExecutionPolicy Bypass -Scope Process`
  - `./Setup/generateNewCertificate.ps1 -name "nameOfSolution" -friendlyName "nameOfSolution" -password "someRandomPassword" -environmentId "758cc81b-8df9-42cb-9d0a-a59482800d1f" -appId "12ec9b01-e104-4af3-b1f5-2ecfc065e1c2"`

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
Use these to control approvals of deployments.

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

### How to create Power Platform service connections with federated credentials
1. Go to Project Settings > Pipelines > Service connections > New service connection > Power Platform
2. Select Workload Identity federation
3. Server URL = The URL of the Dataverse environment (https://dev.crm4.dynamics.com)
4. TenantI Id = Teant Id, can be found in Azure Portal
5. Service Connection Name = The name of the service connetion (e.g. Dataverse Dev)

Once created:
1. Copy the service connection id from the url as it is needed in the next step.
2. Open a new tab and paste https://dev.azure.com/`organization`/`project`/_apis/serviceendpoint/endpoints/`service-connection-id`?api-version=7.1-preview.4
   1. The `organization` and `project` can be found in the URL from ADO
3. Find the `workloadIdentityFederationSubject` in the response and copy it for later.

You now need to create a federated credential on your app registration.
1. Find your app registration in the Azure Portal
2. Go to Manage > Certificates & secrets > Federated credentials > + Add credential
3. For 'Federated credential scenario' select 'Other issuer'
4. Issuer = https://vstoken.dev.azure.com/{organizationName} 
5. Type = Explicit subject identifier
6. Value = Paste the `workloadIdentityFederationSubject` from the earlier step
7. Name = Name of your choice (e.g. PipelineDataverse)

### How to create Azure Resource Manager service connections with federated credentials
1. Go to Project Settings > Pipelines > Service connections > New service connection > Azure Resource Manager
2. Identity type = App registration or managed identity (manual)
3. Credential = Workload identity federation
4. Service Connection Name = The name of the service connetion (e.g. Dev)
5. Directory (tenant) Id = Teant Id, can be found in Azure Portal
6. Click Next
7. Copy the Issuer and Subject Identifier for later use
8. Scope level = Subscription
9. Subcription ID and Subscription Name can be found in the Azure Portal
10. Application (client) ID = The client id of your app registration for the environment.

You now need to create a federated credential on your app registration.
1. Find your app registration in the Azure Portal
2. Go to Manage > Certificates & secrets > Federated credentials > + Add credential
3. For 'Federated credential scenario' select 'Other issuer'
4. Issuer = Paste the issuer (copied in earlier step)
5. Type = Explicit subject identifier
6. Value = Paste the subject (copied in earlier step)
7. Name = Name of your choice (e.g. Pipeline)

## App registration privileges
Remember to give your app reg permission to assign roles.
The easiest method is to add it as owner to the subscription and restrict the role assignment to the ones bicep assigns.
The template uses Storage Queue Data Contributor

## Managed identity
A managed identity is created by the bicep deploy. This is what Azure uses to call back into Dataverse. Make sure it is created as an app user. Search for the client id of the managed identity, you will not find it by name.

## TODO
* Improve validation of infrastructure to be easier to manage
* Auto set storage account environment variable
* Auto user creation for managed identity
