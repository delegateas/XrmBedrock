# XrmBedrock
This project serves as a template for Dataverse projects. This project aims to make it easy to work with Dataverse and Azure together. This project shows that modern development paradigms are applicable to Dataverse.

This template will be updated. The current list is as follows
* Clean-up DataverseService
* Better in-code description of examples to increase adoption.
* New way of handling web resources.
* Deploying data.

# Initial setup
This project serves both as a template, but also as a demonstration of how XrmBedrock can be used. Generated files that are ignored in git are stored for your convenience in Setup/InitialSetup. It is safe to delete that folder.

# Project
Follow these steps to setup the project correctly. After this you are ready to setup Azure Devops.

## Update Plugin Assembly Name
In the Dataverse/src/Plugins.csproj file, update the assembly name from XrmBedrock to the relevant name.

## Update DAXIF Config
Update Solution info and Plugin Assembly name

## Generate new strong name key
Open the developer terminal in Visual Studio and write
`sn -k nameOfSolutionHere.snk`

Update the reference to the .snk file in the Dataverse/src/Plugins.csproj file

## Generate a certificate
Run the Setup/generateNewCertificate.ps1 file in an administrator powershell. Use the following commands.
`Set-ExecutionPolicy Bypass -Scope Process`
`./Setup/generateNewCertificate.ps1 -name "nameOfSolutionHere" -friendlyName "nameOfSolutionHere" -password "someRandomPassword" -environmentId "758cc81b-8df9-42cb-9d0a-a59482800d1f" -appId "12ec9b01-e104-4af3-b1f5-2ecfc065e1c2"`

Set the password in the signing part of the Dataverse/src/Plugins.csproj file

## Update storage account environment variable
Create a new environment variable that will contain the storage account url

Update the name of that variable in DataverseLogic/Azure/AzureConfigSetter

# Devops
## Environment
Create an environment per Dataverse environment. 
Template uses Dev, Test, UAT, Prod.
Use this to control approvals

## Library
Create a variable group per environment.
Template uses Dev, Test, UAT, Prod.
The template assumes the following variables exist.
* ResourceGroupName
* DataverseUrl
* AzureClientId
* AzureClientSecret (only used for DAXIF)
* AzureTenantId (Needed for the managed identity record)
* AzureClientEAObjectId

## Service Connection
A service connection is used to authorize the pipeline against other services. 
The goal is to avoid secrets in the pipeline.
A service connection per azure environment is required.
Use the recommended settings with Workload Federated Credentials.
Template uses Dev, Test, UAT, Prod.

## App reg privileges
Remember to give your app reg permission to assign roles.
The easiest method is to add it as owner to the subscription and restrict the role assignment to the ones bicep assigns.
The template uses Storage Queue Data Contributor

## Managed identity
A managed identity is created by the bicep deploy. This is what azure uses to call back into Dataverse. Make sure it is created as an app user. Search for the client id of the managed identity, you will not find it by name.

## TODO
* Improve validation of infrastructure to be easier to manage
* Auto set storage account environment variable
* Auto user creation for managed identity
