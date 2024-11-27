param name string
param location string

resource userManIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: name
  location: location
}

output userManIdentityId string = userManIdentity.id
output userManIdentityClientId string = userManIdentity.properties.clientId
output userManIdentityPrincipalId string = userManIdentity.properties.principalId
