param env string
param location string
param managedIdentityId string
param kvSecrets array = []
param kvName string
param appServicePlanId string
param keyVaultReferenceIdentity string
param appinsightsConnectionString string
param managedIdentitiesForExternal string
param DataverseUrl string
param userManIdentityClientId string
param appinsightsInstrumentationKey string
param existingAppClientId string
param tenantId string

var defaultAppSettings = [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: appinsightsConnectionString
  }
  {
    name: 'MANAGED_IDENTITIES_FOR_EXTERNAL'
    value: managedIdentitiesForExternal
  }
  {
    name: 'DataverseUrl'
    value: DataverseUrl
  }
  {
    name: 'AZURE_CLIENT_ID'
    value: userManIdentityClientId
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: appinsightsInstrumentationKey
  }
]

var kvSettings = [for secret in kvSecrets: {
  name: secret.name
  value: '@Microsoft.KeyVault(VaultName=${kvName};SecretName=${secret.name})'
}]

var combinedAppSettings = concat(defaultAppSettings, kvSettings)

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: 'app-${toLower(env)}-medlemssystemapi'
  location: location
  kind: 'app'
  properties: {
    keyVaultReferenceIdentity: managedIdentityId
    serverFarmId: appServicePlanId
    siteConfig: {
      alwaysOn: true
      ftpsState:'Disabled'    
      appSettings: combinedAppSettings
      keyVaultReferenceIdentity: keyVaultReferenceIdentity      
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}' : {}
    }
  }
}

resource appServiceAuth 'Microsoft.Web/sites/config@2020-12-01' = {
  parent: appService
  name: 'authsettingsV2'
  properties: {
    platform: {
      enabled: true
    }
    globalValidation: {
      excludedPaths: [
        '/swagger/*'
      ]
      requireAuthentication: true
      redirectToProvider: 'AzureActiveDirectory'
    }
    login: {
      tokenStore: {
        enabled: true
      }
    }
    identityProviders: {
      azureActiveDirectory: {
        enabled: true
        registration: {
          clientId: existingAppClientId
          clientSecretSettingName: 'WEBAPP_CLIENT_SECRET'
          openIdIssuer: 'https://login.microsoftonline.com/${tenantId}/v2.0'
        }
        validation: {
          allowedAudiences: [
            'api://${existingAppClientId}/access_the_api'
          ]
        }
        login: {
          loginParameters: [
            'response_type=code id_token'
            'response_mode=form_post'
            'scope=openid profile email'
          ]
        }
      }
    }
  }
}
