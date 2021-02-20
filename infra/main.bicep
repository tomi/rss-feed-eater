var randomSuffix = '92314' // for globally unique names

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'rssfeedeater${randomSuffix}'
  location: 'westeurope'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
}

output storageId string = storage.id