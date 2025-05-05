param env string = 'Dev'
param existingAppClientObjectId string

// secrets
@secure()
param DataverseUrl string

// Naming (OBS: some resources have limitations to their naming)
var solutionId = 'demo'
var companyId = 'mgs'
var namingSuffix = '${toLower(companyId)}-${toLower(solutionId)}-${toLower(env)}'

// Variables
var location = resourceGroup().location

resource logworkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: 'log-${namingSuffix}'
  location: location
  properties: {
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appinsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'ai-${namingSuffix}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logworkspace.id
    Flow_Type: 'Redfield'
    Request_Source: 'IbizaWebAppExtensionCreate'
    RetentionInDays: 30
  }
}

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: 'ag-${namingSuffix}'
  location: 'global'
  properties: {
    enabled: true
    groupShortName: 'alertgrp'
    emailReceivers: [
    ]
  }
}

resource alertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'alert-exception-${namingSuffix}'
  location: 'global'
  properties: {
    description: 'Alert for exceptions in Function Apps'
    severity: 2
    enabled: true
    scopes: [
      appinsights.id
    ]
    evaluationFrequency: 'PT30M'
    windowSize: 'PT30M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'ExceptionCount'
          metricName: 'exceptions/count'
          timeAggregation: 'Count'
          operator: 'GreaterThan'
          threshold: 0
          criterionType: 'StaticThresholdCriterion'
          metricNamespace: 'microsoft.insights/components'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
    autoMitigate: false
    targetResourceRegion: 'global'
    targetResourceType: 'microsoft.insights/components'
  }
}

module appPlan 'modules/AppServicePlan.bicep' = {
  name: 'appplan'
  params: {
    skuName: 'B1'
    location: location
    name: 'asp-${namingSuffix}'
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-07-01' = {
  name: 'ipvnet-${namingSuffix}'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.24.0.0/24'
      ]
    }
    subnets: [
      {
        name: 'ipsubnet-${namingSuffix}'
        properties: {
          addressPrefix: '172.24.0.0/27'
          natGateway: {
            id: natGateway.id
          }
        }
      }
    ]
  }
}

resource natGateway 'Microsoft.Network/natGateways@2020-07-01' = {
  name: 'ipnat-${namingSuffix}'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    idleTimeoutInMinutes: 4
    publicIpAddresses: [
      {
        id: publicIp.id
      }
    ]
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2020-07-01' = {
  name: 'ip-${namingSuffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Regional'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    publicIPAddressVersion: 'IPv4'
  }
}

module userManangedIdentityModule 'modules/UserManagedIdentity.bicep' = {
  name: 'userManangedIdentityModule'
  params: {
    name: 'mi-${namingSuffix}'
    location: location
  }
}

module principals 'principals.bicep' = { 
  name: 'principal'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'sa${padLeft(replace(namingSuffix, '-', ''), 3, 'x')}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: true
    isHnsEnabled: true
    accessTier: 'Cool'
  }
}

var storageQueueContributorRoleId = '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount
  name: guid(storageAccount.id, existingAppClientObjectId, storageQueueContributorRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageQueueContributorRoleId)
    principalId: existingAppClientObjectId
    principalType: 'ServicePrincipal'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'asp-${namingSuffix}'
  location: location
  sku: {
    name: 'S1'
  }
  kind: 'app'
  properties: {
    reserved: false
  }
}

var kvPrincipals = concat(principals.outputs.userprincipals, [
  userManangedIdentityModule.outputs.userManIdentityPrincipalId
])

var keyvaultName = 'kv-${namingSuffix}'

module keyvault 'modules/keyVault.bicep' = {
  name: 'keyvault'
  params: {
    name: keyvaultName
    location: location
    principalIds: kvPrincipals
  }
}

var dataverseSecrets = [
]

module kvSecrets 'modules/keyVaultSecrets.bicep' = {
  name: 'keyVaultSecrets'
  params: {
    keyVaultName: keyvault.outputs.keyvaultname
    secrets: concat(dataverseSecrets, [
      { name: 'StorageAccountName', value: storageAccount.name }
    ])
  }
}

var functionApps = [
  {
    functionAppName: 'func-economyarea-${namingSuffix}'
    kvSecrets: dataverseSecrets
    funcAppSettings: []
    functionAppVersion: null
    staticIp: false
    externalPackageUrl: null
    isolated: true
  }
]

module functionApp 'modules/functionApps.bicep' = [for app in functionApps: {
  name: app.functionAppName
  params: {
    location: location
    managedIdentityId: userManangedIdentityModule.outputs.userManIdentityId
    storageAccountConnectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
    functionAppName: app.functionAppName
    functionAppVersion: app.functionAppVersion
    InstrumentationKey: appinsights.properties.InstrumentationKey
    appInsightsConnectionString: appinsights.properties.ConnectionString
    appSettings: app.funcAppSettings
    keyVaultReferenceIdentity: userManangedIdentityModule.outputs.userManIdentityPrincipalId
    managedIdentityClientId: userManangedIdentityModule.outputs.userManIdentityClientId
    dataverseUrl: DataverseUrl
    planId: appPlan.outputs.id
    kvSecrets: app.kvSecrets
    kvName: keyvault.outputs.keyvaultname
    subnetId: app.staticIp ? vnet.properties.subnets[0].id : null
    packageUri: app.externalPackageUrl
    workerRuntime: app.isolated ? 'dotnet-isolated' : 'dotnet'
    netFrameworkVersion: app.isolated ? 'v8.0' : 'v6.0'
  }
}]

var storageQueues = [
  'createinvoicesqueue'
]

module storagequeue 'modules/StorageQueue.bicep' = [for queue in storageQueues: {
  name: queue
  params: {
    storageaccountname: storageAccount.name
    storageaccountqueuename: queue
  }
}]
