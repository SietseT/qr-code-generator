@description('Location for all resources.')
param location string = resourceGroup().location

@description('Common name for all resources.')
param commonName string = 'qrgenerator'

@description('Environment short name')
param environment string = 'dev'

@description('Resource name with environment')
param resourceName string = '${environment}-${commonName}'

@description('Function URL of QR API entrypoint')
param functionApiUrl string


@description('Function URL of SignalR hub')
param functionHubUrl string


var buildNumber = uniqueString(resourceGroup().id)

//----------- SignalR Deployment ------------
module signalR 'signalr.bicep' = {
  name: 'srdeploy-${buildNumber}'
  params: {
    name: 'sr-${resourceName}'
    location: location
    upstream: functionHubUrl
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
module functionAppServicePlan 'serviceplan.bicep' = {
  name: 'funcplandeploy-${buildNumber}'
  params: {
    name: 'asp-${resourceName}'
    location: location
    kind: 'functionapp'
    os: 'Linux'
    sku: {
      name: 'Y1'
    }
  }
}

//----------- App Service Plan Deployment ------------
module webServicePlan 'serviceplan.bicep' = {
  name: 'webplandeploy-${buildNumber}'
  params: {
    name: 'asp-${resourceName}-free'
    location: location
    kind: 'windows'
    os: 'Windows'
    sku: {
      name: 'F1'
      tier: 'Free'
      size: 'F1'
      family: 'F'
      capacity: 1
    }
  }
}

//----------- Function App: API ---------------------
module functionAppApi 'function-api.bicep' = {
  name: 'functiondeployapi-${buildNumber}'
  params: {
    name: 'func-${resourceName}-api'
    location: location
    planId: functionAppServicePlan.outputs.planId
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
    functionAppServicePlan
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
    planId: functionAppServicePlan.outputs.planId
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
    functionAppServicePlan
    storageAccount
    serviceBus
  ]
}

//----------- Function App: Hub ---------------------
module functionAppHub 'function-api.bicep' = {
  name: 'functiondeployhub-${buildNumber}'
  params: {
    name: 'func-${resourceName}-hub'
    location: location
    planId: functionAppServicePlan.outputs.planId
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
    appSettings: {
      Values__QrApiUrl: functionApiUrl
    }
  }
  dependsOn: [
    functionAppServicePlan
    storageAccount
    signalR
  ]
}

//----------- Web app ---------------------
module webApp 'webapp.bicep' = {
  name: 'webdeploy-${buildNumber}'
  params: {
    name: 'app-${resourceName}'
    location: location
    planId: webServicePlan.outputs.planId
  }
  dependsOn: [
    webServicePlan
  ]
}
