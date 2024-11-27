param functionAppName string
param appSettings array = []
param kvSecrets array = []
param kvName string
param keyVaultReferenceIdentity string
param storageAccountConnectionString string
param functionAppVersion string = '4'
param InstrumentationKey string
param location string
param appInsightsConnectionString string
param managedIdentityId string
param managedIdentityClientId string
param dataverseUrl string
param planId string
param subnetId string = ''
param packageUri string = '1'
param workerRuntime string = 'dotnet'
param netFrameworkVersion string = 'v8.0'

var defaultAppSettings = [
  {
    name: 'AzureWebJobsStorage'
    value: storageAccountConnectionString
  }
  {
    name: 'FuncAcStorageConnectionAppSetting'
    value: storageAccountConnectionString
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: workerRuntime
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~${functionAppVersion}'
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: InstrumentationKey
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: appInsightsConnectionString
  }
  {
    name: 'WEBSITE_ENABLE_SYNC_UPDATE_SITE'
    value: 'true'
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: packageUri
  }
  {
    name: 'AZURE_CLIENT_ID'
    value: managedIdentityClientId
  }
  {
    name: 'DataverseUrl'
    value: dataverseUrl
  }
]

var staticOutputIpSettings = [
  {
    name: 'WEBSITE_VNET_ROUTE_ALL'
    value: '1'
  }
]

var kvSettings = [for secret in kvSecrets: {
  name: secret.name
  value: '@Microsoft.KeyVault(VaultName=${kvName};SecretName=${secret.name})'
}]

var combinedAppSettings = concat(defaultAppSettings, appSettings, kvSettings, subnetId != null ? staticOutputIpSettings : [])

resource azureFunction 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}' : {}
    }
  }
  properties: {
    vnetRouteAllEnabled: !empty(subnetId)
    keyVaultReferenceIdentity: managedIdentityId
    serverFarmId: planId
    publicNetworkAccess: 'Enabled'
    siteConfig: {
      alwaysOn: true
      ftpsState:'Disabled'
      netFrameworkVersion: netFrameworkVersion
      use32BitWorkerProcess: netFrameworkVersion != 'v8.0'
      appSettings: combinedAppSettings
      keyVaultReferenceIdentity: keyVaultReferenceIdentity
      ipSecurityRestrictions: [
        {
          ipAddress: 'Any'
          action: 'Allow'
          priority: 2147483647
          name: 'Allow all'
          description: 'Allow pushing of new code but no trigger'
          vnetSubnetResourceId: empty(subnetId) ? null : subnetId
        }
      ]
    }
  }
}

output azureFunctionId string = azureFunction.id
