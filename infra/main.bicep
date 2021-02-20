param apiPublisherEmail string
param tenantId string

var randomSuffix = '92314' // for globally unique names
var location = resourceGroup().location

// Storage account for Azure functions
resource functionsStorage 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: 'rssfeedeater${randomSuffix}funcs'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
}

// Log analytics workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'rssfeedeater-loganalytics'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

// Application insights
resource appinsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: 'rssfeedeater-appinsights'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

// Key vault
resource keyvault 'Microsoft.KeyVault/vaults@2020-04-01-preview' = {
  name: 'rssfeedeater-keyvault'
  location: location
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: tenantId
  }
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

// Cosmos DB
resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'rssfeedeater${randomSuffix}-cosmosdb'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    createMode: 'Default'
    locations: [
      {
        locationName: 'West Europe'
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// API Management
// resource apiManagement 'Microsoft.ApiManagement/service@2020-06-01-preview' = {
//   name: 'rssfeedeater-api-management'
//   location: location
//   sku: {
//     name: 'Consumption'
//     capacity: 0
//   }
//   identity: {
//     type: 'SystemAssigned'
//   }
//   properties: {
//     publisherEmail: apiPublisherEmail
//     publisherName: 'RSS Feed Eater'
//     customProperties: {
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls11': 'false'
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls10': 'false'
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls11': 'false'
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls10': 'false'
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Ssl30': 'false'
//       'Microsoft.WindowsAzure.ApiManagement.Gateway.Protocols.Server.Http2': 'false'
//     }
//   }
// }

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