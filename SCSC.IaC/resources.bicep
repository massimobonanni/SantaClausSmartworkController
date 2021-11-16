param environmentName string ='demo' 
param location string = resourceGroup().location

var dataStorageName = 'scsc${environmentName}datastore'
var funcStorageName ='scsc${environmentName}funcstore'
var appInsightsName='scsc${environmentName}appinsight'
var funcAppName= 'scsc${environmentName}api'
var funcAppPlanName= 'scsc${environmentName}appplan'
var adminAppName= 'scsc${environmentName}admin'
var elfAppName= 'scsc${environmentName}elf'
var webAppPlanName= 'scsc${environmentName}webappplan'

resource dataStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name:dataStorageName
  sku: {
    name: 'Standard_LRS'
  }
}

resource functionStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name: funcStorageName
  sku: {
    name: 'Standard_LRS'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: { 
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource functionAppPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: funcAppPlanName
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: funcAppName
  location: location
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: functionAppPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      cors: {
        allowedOrigins : [
          '*'
        ] 
      }
      appSettings: [
        {
          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
      ]
    }
  }
}

resource webAppPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: webAppPlanName
  location: location
  sku: {
    name: 'F1' 
    tier: 'Free'
  }
}

resource adminAppService 'Microsoft.Web/sites@2021-02-01' = {
  name: adminAppName
  location: location
  properties: {
    serverFarmId: webAppPlan.id
    siteConfig:{
      netFrameworkVersion: '5.0'
    }
  }
}

resource elfAppService 'Microsoft.Web/sites@2021-02-01' = {
  name: elfAppName
  location: location
  properties: {
    serverFarmId: webAppPlan.id
    siteConfig:{
      netFrameworkVersion: '5.0'
    }
  }
}
