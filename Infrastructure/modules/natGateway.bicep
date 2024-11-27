param name string
param location string = resourceGroup().location
param tags object
param ipId string

resource nat 'Microsoft.Network/natGateways@2020-07-01' = {
  name: name
  tags: tags
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    idleTimeoutInMinutes: 4
    publicIpAddresses: [
      {
        id: ipId
      }
    ]
  }
}

output id string = nat.id
