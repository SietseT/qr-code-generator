@description('Location for all resources.')
param location string = resourceGroup().location

@description('Common name for all resources.')
param commonName string = 'qrgenerator'

@description('Environment short name')
param environment string = 'dev'

@description('Resource name with environment')
param resourceName string = '${environment}-${commonName}'


@description('Generated from /subscriptions/74faf385-f368-4570-81a5-318a7d23c16a/resourceGroups/reg-dev-qrgenerator/providers/Microsoft.SignalRService/SignalR/sr-dev-qrgenerator')
resource signalr 'Microsoft.SignalRService/signalR@2022-02-01' = {
  sku: {
    name: 'Free_F1'
    tier: 'Free'
    capacity: 1
  }
  properties: {
    tls: {
      clientCertEnabled: false
    }
    features: [
      {
        flag: 'ServiceMode'
        value: 'Serverless'
        properties: {}
      }
      {
        flag: 'EnableConnectivityLogs'
        value: 'True'
        properties: {}
      }
    ]
    resourceLogConfiguration: {
      categories: [
        {
          name: 'ConnectivityLogs'
          enabled: 'true'
        }
        {
          name: 'MessagingLogs'
          enabled: 'false'
        }
        {
          name: 'HttpRequestLogs'
          enabled: 'false'
        }
      ]
    }
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    upstream: { }
    networkACLs: {
      defaultAction: 'Deny'
      publicNetwork: {
        allow: [
          'ServerConnection'
          'ClientConnection'
          'RESTAPI'
          'Trace'
        ]
      }
      privateEndpoints: []
    }
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    disableAadAuth: false
  }
  kind: 'SignalR'
  location: location
  tags: {}
  name: 'sr-${resourceName}'
}

var buildNumber = uniqueString(resourceGroup().id)

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
    serviceBusConnectionString: serviceBus.outputs.sendConnectionString
  }
  dependsOn: [
    storageAccount
    serviceBus
  ]
}
