param storageaccountname string 
param storageaccountqueuename string 

resource storageaccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = { name:storageaccountname }

resource service 'Microsoft.Storage/storageAccounts/queueServices@2022-05-01' = {
  parent: storageaccount
  name: 'default'
}

resource queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2022-05-01' = {
  parent: service
  name: storageaccountqueuename
}
