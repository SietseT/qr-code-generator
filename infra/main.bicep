@description('Location for all resources.')
param location string = resourceGroup().location

@description('Common name for all resources.')
param commonName string = 'qrgenerator'

@description('Environment short name')
param environment string = 'dev'

@description('Resource name with environment')
param resourceName string = '${environment}-${commonName}'




var buildNumber = uniqueString(resourceGroup().id)

//----------- SignalR Deployment ------------
module signalR 'signalr.bicep' = {
  name: 'srdeploy-${buildNumber}'
  params: {
    name: 'sr-${resourceName}'
    location: location
  }
}

//----------- Storage Account Deployment ------------
module storageAccount 'storageaccount.bicep' = {
  name: 'stvmdeploy-${buildNumber}'
  params: {
    name: 'sa${environment}${commonName}'
    location: location
  }
}

//----------- Service Bus Deployment ------------
module serviceBus 'servicebus.bicep' = {
  name: 'sbdeploy-${buildNumber}'
  params: {
    name: 'sb-${resourceName}'
    location: location
    sku: 'Basic'
  }
}

//----------- App Service Plan Deployment ------------
module servicePlan 'serviceplan.bicep' = {
  name: 'plandeploy-${buildNumber}'
  params: {
    name: 'asp-${resourceName}'
    location: location
    os: 'Linux'
  }
}

//----------- Function App: API ---------------------
module functionAppApi 'function-api.bicep' = {
  name: 'functiondeployapi-${buildNumber}'
  params: {
    name: 'func-${resourceName}-api'
    location: location
    planId: servicePlan.outputs.planId
    functionAppRuntime: 'dotnet'
    storageAccountConnectionString: storageAccount.outputs.connectionString
    connectionStrings: {
      StorageAccount: {
        value: storageAccount.outputs.connectionString
        type: 'Custom'
      }
      ServiceBus: {
        value: serviceBus.outputs.sendConnectionString
        type: 'Custom'
      }
    }
  }
  dependsOn: [
    storageAccount
    serviceBus
  ]
}

//----------- Function App: Generators ---------------------
module functionAppGenerators 'function-api.bicep' = {
  name: 'functiondeploygenerators-${buildNumber}'
  params: {
    name: 'func-${resourceName}-generators'
    location: location
    planId: servicePlan.outputs.planId
    functionAppRuntime: 'dotnet'
    storageAccountConnectionString: storageAccount.outputs.connectionString
    connectionStrings: {
      StorageAccount: {
        value: storageAccount.outputs.connectionString
        type: 'Custom'
      }
      ServiceBus: {
        value: serviceBus.outputs.readConnectionString
        type: 'Custom'
      }
    }
  }
  dependsOn: [
    storageAccount
    serviceBus
  ]
}

//----------- Function App: Generators ---------------------
module functionAppHub 'function-api.bicep' = {
  name: 'functiondeployhub-${buildNumber}'
  params: {
    name: 'func-${resourceName}-hub'
    location: location
    planId: servicePlan.outputs.planId
    functionAppRuntime: 'dotnet'
    storageAccountConnectionString: storageAccount.outputs.connectionString
    connectionStrings: {
      StorageAccount: {
        value: storageAccount.outputs.connectionString
        type: 'Custom'
      }
      AzureSignalRConnectionString: {
        value: signalR.outputs.connectionString
        type: 'Custom'
      }
    }
  }
  dependsOn: [
    storageAccount
    signalR
  ]
}
