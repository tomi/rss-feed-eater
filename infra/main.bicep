resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'rss-feed-eater-92314'
  location: 'westeurope'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
}

output storageId string = storage.id