@description('SignalR name')
param name string

@description('SignalR location')
param location string

@description('Upstream hub URL')
param upstream string

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
    upstream: {
      templates: [
        {
          hubPattern: '*'
          eventPattern: '*'
          categoryPattern: '*'
          urlTemplate: upstream
          auth: {
            type: 'None'
          }
        }
      ]
    }
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
  name: name
  location: location
  tags: {}
}

output connectionString string = listKeys(signalr.id, signalr.apiVersion).primaryConnectionString


