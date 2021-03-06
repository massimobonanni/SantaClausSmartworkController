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
var keyVaultName= 'scsc${environmentName}kv'

resource dataStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name:dataStorageName
  sku: {
    name: 'Standard_LRS'
  }
}

var dataStorageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${dataStorage.name};AccountKey=${listKeys(dataStorage.id, dataStorage.apiVersion).keys[0].value}'

resource functionStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name: funcStorageName
  sku: {
    name: 'Standard_LRS'
  }
}

var functionStorageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'

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

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01'= {
  location:location
  name: keyVaultName
  properties:{
    sku: {
      name: 'standard'
      family:  'A'
    }
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: true
    enablePurgeProtection: true
    vaultUri: 'https://${keyVaultName}.vault.azure.net/'
  }

  resource dataStorageConnectionStringSecret 'secrets' = {
    name: 'PackagesStorageAccount'
    properties: {
      value: dataStorageConnectionString
    }
  }

  resource functionStorageConnectionStringSecret 'secrets' = {
    name: 'AzureWebJobsStorage'
    properties: {
      value: functionStorageConnectionString
    }
  }

  resource sendGridApiKeySecret 'secrets' = {
    name: 'SendGridApiKey'
    properties: {
      value: ''
    }
  }

  resource twilioAccountSidSecret 'secrets' = {
    name: 'TwilioAccountSid'
    properties: {
      value: ''
    }
  }

  resource twilioAuthTokenSecret 'secrets' = {
    name: 'TwilioAuthToken'
    properties: {
      value: ''
    }
  }

  resource functionAppDefaultCodeSecret 'secrets' = {
    name: 'FunctionAppDefaultCode'
    properties: {
      value: ''
    }
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
  identity:{
    type: 'SystemAssigned'
  }
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
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::functionStorageConnectionStringSecret.properties.secretUri})'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name: 'PackagesStorageAccount'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::dataStorageConnectionStringSecret.properties.secretUri})'
        }
        {
          name: 'SendGridApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::sendGridApiKeySecret.properties.secretUri})'
        }
        {
          name: 'EmailNotificationFrom'
          value: 'noreply@scsc.com'
        }
        {
          name: 'TwilioAccountSid'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::twilioAccountSidSecret.properties.secretUri})'
        }
        {
          name: 'TwilioAuthToken'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::twilioAuthTokenSecret.properties.secretUri})'
        }
        {
          name: 'TwilioFromNumber'
          value: '+13253350391'
        }
        {
          name: 'PurgeHistoryTimer'
          value: '0 0 * * * *'
        }
        {
          name: 'PurgeRetentionInDays'
          value: '7'
        }
      ]
    }
  }
  dependsOn:[
    keyVault
  ]
}

resource functionAppKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('Key Vault Secret User', funcAppName, subscription().subscriptionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // this is the role "Key Vault Secrets User"
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    keyVault
    functionApp
  ]
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
  identity:{
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: webAppPlan.id
    siteConfig:{
      netFrameworkVersion: '5.0'
      appSettings:[
        {
          name: 'APISettings:BaseUrl'
          value: 'https://${functionApp.properties.defaultHostName}'
        }
        {
          name: 'APISettings:ApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault::functionAppDefaultCodeSecret.properties.secretUri})'
        }
      ]
    }
  }
  dependsOn:[
    keyVault
  ]
}

resource adminAppServiceKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('Key Vault Secret User', adminAppName, subscription().subscriptionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // this is the role "Key Vault Secrets User"
    principalId: adminAppService.identity.principalId
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    keyVault
    functionApp
  ]
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
