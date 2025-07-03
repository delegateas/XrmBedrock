# XrmBedrock
This project serves as a template for Dataverse projects. This project aims to make it easy to work with Dataverse and Azure together. This project shows that modern development paradigms are applicable to Dataverse.

This template will be updated. The current list is as follows
* Clean-up DataverseService
* Better in-code description of examples to increase adoption.
* New way of handling web resources.
* Deploying data.

# Initial setup
This project serves both as a template, but also as a demonstration of how XrmBedrock can be used. Generated files that are ignored in git are stored for your convenience in Setup/InitialSetup. It is safe to delete that folder.

Right after downloading XrmBedrock, you are not able to build the project since it is also used as a demonstration. In order to build, you should first run ``Setup/copyInitialSetup.ps1`` using PowerShell. The script is not signed, so make sure to first run ``Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass``. This copies the context for the demo and you should now be able to build and run all unit tests for the demo. 

# Getting up and running
Follow these steps to setup your project correctly. After this you are ready to setup Azure DevOps.

# Rename file names and folders
- Rename file ``XrmBedrock.sln``
- Rename file ``xrmbedrock.snk``
- Rename folder ``src/Dataverse/Webresources/src/mgs_Magesoe``

# Update values in WebResources files
- Update ``mgs_Magesoe`` to the new folder name in ``src/Dataverse/src/<new folder name>/esbuild.config.mjs``
- Update ``mgs_Magesoe`` to the new folder name in ``src/Dataverse/src/<new folder name>/package.json``
- Update ``mgs_Magesoe`` to the new folder name in ``src/Dataverse/src/<new folder name>/tsconfig.json``

## Update values in Plugins.csproj
In the ``src/Dataverse/Plugins.csproj`` file, update the following:
- RootNameSpace
- AssemblyName
- AssemblyOriginatorKeyFile
- Reference to the .snk file in the ``Exec`` element

## Update DAXIF Config
In the ``src\Tools\Daxif\_Config.fsx`` file, update/configure the following:
- SolutionInfo
- PublisherInfo
- Environments
  - url
  - the url part of the connectionString
  - The pipeline expects environment names Dev, Test, UAT and Prod - make sure that the names of the environment matches what the pipeline excepts, modify it if needed. 
- ``src\Tools\Daxif\GenerateDataverseDomain.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\GenerateTypeScriptContext.fsx``
  - add or remove table names based on your solution and needs
- ``src\Tools\Daxif\username.txt``
  - add your username

## Generate new strong name key
Open the developer terminal in Visual Studio and write: 
`sn -k nameOfSolution.snk`

Make sure you have updated the reference to the .snk file in ``src/Dataverse/Plugins.csproj``.

## Generate a certificate
TODO: Add a description of what the certificate is used for.

- Create a new password (at least 12 randomly generated characters is recommended). 
- Open an administrator powershell and run the ``Setup/generateNewCertificate.ps1`` file. 
- Use the following commands - remember to update "nameOfSolution" and "someRandomPassword":
  - `Set-ExecutionPolicy Bypass -Scope Process`
  - `./Setup/generateNewCertificate.ps1 -name "nameOfSolution" -friendlyName "nameOfSolution" -password "someRandomPassword" -environmentId "758cc81b-8df9-42cb-9d0a-a59482800d1f" -appId "12ec9b01-e104-4af3-b1f5-2ecfc065e1c2"`

Set the password in the signing part of the ``src/Dataverse/Plugins.csproj`` file in the ``Exec`` element.

## Update storage account environment variable
Create a new environment variable that will contain the storage account url

Update the reference to that variable in ``src\Dataverse\SharedPluginLogic\Logic\Azure\AzureConfigSetter.cs`` 

## Verify solution and delete demo code
At this point you should still be able to build and run all unit tests. 
As soon as you run ``GenerateCSharpContext.fsx`` you will get a lot of errors, since the XrmContext will be overwritten and replaced with your solution's context. In order to build again, you need to delete all the demo code...

Remember to delete the Setup folder as well. 

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


# GitHub
Additionally, this section is relevant if the code repository is hosted and GitHub and you want to use GitHub Actions.

_NB: Only the dataverse parts are converted from Azure DevOps Pipelines to GitHub Actions_.

## Environment

The workflows assume a base environment named `dev` is created with variables `DATAVERSE_APP_ID` and `TENANT_ID` and secrets `CLIENT_SECRET`. The base environemnt URL must be updated in the workflow defintion and the environment specific URLs are constructed from the GitHub environment name.

Remember to "uncomment" the `build-and-test.yaml` workflow trigger.


## TODO
* Improve validation of infrastructure to be easier to manage
* Auto set storage account environment variable
* Auto user creation for managed identity
