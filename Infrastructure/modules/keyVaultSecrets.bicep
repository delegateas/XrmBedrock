@description('Specifies the name of the key vault.')
param keyVaultName string

@description('An array of json objects like this : {\'name\':name, \'value\':value}')
param secrets array

resource keyvaultsecrets 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = [for secret in secrets: {
  name: '${keyVaultName}/${secret.name}'
  properties: {
    value: secret.value
  }
}]
