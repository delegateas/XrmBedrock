param name string
param location string
param principalIds array

var accessPolicies = [for principalId in principalIds: {
  tenantId: subscription().tenantId
  objectId: principalId
  permissions: {
    keys: [
      'get'
      'list'
      'wrapKey'
      'unwrapKey'
    ]
    secrets: [
      'get'
      'list'
    ]
    certificates: []
  }
}]

resource kv 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: name
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: accessPolicies
  }
}

output keyvaultname string = kv.name
