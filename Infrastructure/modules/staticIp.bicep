param name string
param location string = resourceGroup().location
param tags object

resource ip 'Microsoft.Network/publicIPAddresses@2020-07-01' = {
  name: name
  tags: tags
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

output id string = ip.id
