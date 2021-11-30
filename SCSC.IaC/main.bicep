// This template creates resource group and use a module to create resources
targetScope = 'subscription'

param environmentName string = 'demo' 
param location string = deployment().location

var resourceGroupName = 'SCSC-${environmentName}-rg'


resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'resources'
  params: {
    location : location
    environmentName: environmentName
  }
}
