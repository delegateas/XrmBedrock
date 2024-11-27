param location string = resourceGroup().location
param env string = 'dev'
param solutionId string = 'msys'
param companyId string = 'lf'
param existingAppClientId string
param existingAppClientObjectId string
param getPaymentsFromFarpayDisabled bool
param managedIdentitiesForExternal string

// secrets
@secure()
param DataverseUrl string
@secure()
param DataverseClientId string
@secure()
param DataverseClientSecret string
@secure()
param CvrUsername string
@secure()
param CvrPassword string
@secure()
param WebAppClientSecret string
@secure()
param FarPayApiKey string
@secure()
param DMAzureSqlConnectionString string

// CPR Secrets
param cprDisabled bool
param cprKey string
param cprCustomerNumber string
param cprConnectionString string
@secure()
param cprFTPUser string
@secure()
param cprFTPPassword string
param cprDeployPackageUrl string

// Naming
var namingSuffix = '${toLower(companyId)}-${toLower(solutionId)}-${toLower(env)}'
var tenantId = subscription().tenantId

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
      {
        name: 'Jan'
        emailAddress: 'sse@delegate.dk'
        useCommonAlertSchema: true
      }
      {
        name: 'Morten'
        emailAddress: 'mzo@delegate.dk'
        useCommonAlertSchema: true
      }
      {
        name: 'Osman'
        emailAddress: 'ose@delegate.dk'
        useCommonAlertSchema: true
      }
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
  name: 'sa${uniqueString(resourceGroup().id)}'
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

module appServiceModule 'modules/appService.bicep' = {
  name: 'appServiceModule'
  params: {
    env: env
    location: location
    managedIdentityId: userManangedIdentityModule.outputs.userManIdentityId
    appServicePlanId: appServicePlan.id
    kvSecrets: concat(farPaySecrets, dataverseSecrets, webAppSecrets)
    kvName: keyvault.outputs.keyvaultname
    keyVaultReferenceIdentity: userManangedIdentityModule.outputs.userManIdentityPrincipalId
    appinsightsConnectionString: appinsights.properties.ConnectionString
    managedIdentitiesForExternal: managedIdentitiesForExternal
    DataverseUrl: DataverseUrl
    userManIdentityClientId: userManangedIdentityModule.outputs.userManIdentityClientId
    appinsightsInstrumentationKey: appinsights.properties.InstrumentationKey
    existingAppClientId: existingAppClientId
    tenantId: tenantId
  }
}

var kvPrincipals = concat(principals.outputs.userprincipals, [
  userManangedIdentityModule.outputs.userManIdentityPrincipalId
])

var keyvaultName = 'keyvault-${namingSuffix}'

module keyvault 'modules/keyVault.bicep' = {
  name: 'keyvault'
  params: {
    name: keyvaultName
    location: location
    principalIds: kvPrincipals
  }
}

var cvrSecrets = [
  {
    name: 'CvrUrl'
    value: 'http://distribution.virk.dk/cvr-permanent/virksomhed/_search'
  }
  {
    name: 'CvrUsername'
    value: CvrUsername
  }
  {
    name: 'CvrPassword'
    value: CvrPassword
  }
]

var dataverseSecrets = [
]

var authRegSecrets = [
  {
    name: 'AuthRegBaseUrl'
    value: 'https://autregweb.sst.dk/Authorization.aspx'
  }
]

var webAppSecrets = [
  {
    name: 'WebAppClientSecret'
    value: WebAppClientSecret
  }
]

var cprFtpPasswordDef = {
  name: 'CprFtpPassword'
  value: cprFTPPassword
}

var cprCrmClientDef = {
  name: 'CprCrmClient'
  value: DataverseClientId
}

var cprCrmClientSecretDef = {
  name: 'CprCrmClientSecret'
  value: DataverseClientSecret
}

var cprFtpConnectionStringDef = {
  name: 'CprFtpConnectionString'
  value: cprConnectionString
}

var cprSecrets = [
  cprFtpPasswordDef
  cprCrmClientDef
  cprCrmClientSecretDef
  cprFtpConnectionStringDef
]

var farPaySecrets = [
  {
    name: 'FarPayUrl'
    value: 'https://api.farpay.io'
  }
  {
    name: 'FarPayApiKey'
    value: FarPayApiKey
  }
]

var dmAzureSqlSecrets = [
  {
    name: 'DMAzureSqlConnectionString'
    value: DMAzureSqlConnectionString
  }
]

module kvSecrets 'modules/keyVaultSecrets.bicep' = {
  name: 'keyVaultSecrets'
  params: {
    keyVaultName: keyvault.outputs.keyvaultname
    secrets: concat(cvrSecrets, dataverseSecrets, authRegSecrets, cprSecrets, webAppSecrets, farPaySecrets, dmAzureSqlSecrets, [
      { name: 'StorageAccountName', value: storageAccount.name }
    ])
  }
}

var functionApps = [
  {
    functionAppName: 'func-${toLower(env)}-committeearea'
    kvSecrets: dataverseSecrets
    funcAppSettings: []
    functionAppVersion: null
    staticIp: false
    externalPackageUrl: null
    isolated: true
  }
  {
    functionAppName: 'func-${toLower(env)}-cpr'
    kvSecrets: dataverseSecrets
    funcAppSettings: [
      {
        name: 'CPR_DISABLED'
        value: cprDisabled
      }
      {
        name: 'CPR_FTP_USER'
        value: cprFTPUser
      }
      {
        name: 'CPR_KEY'
        value: cprKey
      }
      {
        name: 'CPR_CUSTOMER_NUMBER'
        value: cprCustomerNumber
      }
      {
        name: 'CPR_CRM_URI'
        value: DataverseUrl
      }
      {
        name: 'WEBSITE_TIME_ZONE'
        value: 'Central Europe Standard Time'
      }
      {
        name: 'CPR_FTP_PASSWORD'
        value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${cprFtpPasswordDef.name})'
      }
      {
        name: 'CPR_CRM_CLIENTID'
        value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${cprCrmClientDef.name})'
      }
      {
        name: 'CPR_CRM_CLIENTSECRET'
        value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${cprCrmClientSecretDef.name})'
      }
      {
        name: 'CPR_FTP_CONNECTIONSTRING'
        value: '@Microsoft.KeyVault(VaultName=${keyvaultName};SecretName=${cprFtpConnectionStringDef.name})'
      }
    ]
    functionAppVersion: '1'
    staticIp: true
    externalPackageUrl: cprDeployPackageUrl
    isolated: false
  }
  {
    functionAppName: 'func-${toLower(env)}-customerarea'
    kvSecrets: concat(authRegSecrets, cvrSecrets, dataverseSecrets, dmAzureSqlSecrets)
    funcAppSettings: []
    functionAppVersion: null
    staticIp: true
    externalPackageUrl: null
    isolated: true
  }
  {
    functionAppName: 'func-${toLower(env)}-economyarea'
    kvSecrets: concat(farPaySecrets, dataverseSecrets)
    funcAppSettings: [
      {
      name: 'AzureWebJobs.GetFarPayPayments.Disabled'
      value: getPaymentsFromFarpayDisabled
      }
    ]
    functionAppVersion: null
    staticIp: false
    externalPackageUrl: null
    isolated: true
  }
  {
    functionAppName: 'func-${toLower(env)}-employmentarea'
    kvSecrets: dataverseSecrets
    funcAppSettings: []
    functionAppVersion: null
    staticIp: false
    externalPackageUrl: null
    isolated: true
  }
  {
    functionAppName: 'func-${toLower(env)}-productarea'
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
  'authregqueue'
  'createfarpaycustomerqueue'
  'createinvoicesqueue'
  'cvrupdatequeue'
  'disbursementqueue'
  'hierarchycalculatorqueue'
  'recalculateplacementqueue'
  'recalculateunionlookupqueue'
  'updateaccountqueue'
  'sendinvoicequeue'
]

module storagequeue 'modules/StorageQueue.bicep' = [for queue in storageQueues: {
  name: queue
  params: {
    storageaccountname: storageAccount.name
    storageaccountqueuename: queue
  }
}]
