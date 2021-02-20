var randomSuffix = '92314' // for globally unique names
var location = resourceGroup().location

// Storage account for Azure functions
resource functionsStorage 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: 'rssfeedeater${randomSuffix}functions'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
}

// Application insights
resource appinsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: 'rssfeedeater-appinsights'
  location: location
  kind: 'web'
}

// Hosting plan for the function app
resource functionAppHost 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: 'rssfeedeater${randomSuffix}-plan'
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: 'rssfeedeater${randomSuffix}'
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: functionAppHost.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionsStorage.name};AccountKey=${listKeys(functionsStorage.id, '2020-08-01-preview').primaryKey}'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appinsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'node'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '14'
        }
      ]
    }
  }
}

// Storage account for CDN backend
resource cdnStorage 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: 'rssfeedeater${randomSuffix}cdn'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
}

output functionsStorageId string = functionsStorage.id
output cdnStorageId string = cdnStorage.id