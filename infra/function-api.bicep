@description('Function App name')
param name string

@description('Function App location')
param location string

@description('App Service Plan Id')
param planId string

@description('Function App runtime')
@allowed([
  'dotnet'
  'node'
  'python'
  'java'
])
param functionAppRuntime string

@description('Connection strings')
@secure()
param connectionStrings object

@description('Storage Account connection string')
@secure()
param storageAccountConnectionString string

var function_extension_version = '~4'

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: planId
  }
}

resource functionAppSettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${name}/appsettings'
  properties: {
    AzureWebJobsStorage: storageAccountConnectionString
    FUNCTIONS_EXTENSION_VERSION: function_extension_version
    FUNCTIONS_WORKER_RUNTIME: functionAppRuntime
  }
  dependsOn: [
    functionApp
  ]
}

resource functionAppConnectionStrings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${name}/connectionstrings'
  properties: connectionStrings
  dependsOn: [
    functionApp
  ]
}


output functionAppName string = functionApp.name
