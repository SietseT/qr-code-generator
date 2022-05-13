@description('Location for all resources.')
param location string = resourceGroup().location

@description('Common name for all resources.')
param commonName string = 'qrgenerator'

@description('Environment short name')
param environment string = 'dev'

@description('Resource name with environment')
param resourceName string = '${environment}-${commonName}'

@description('Generated from /subscriptions/74faf385-f368-4570-81a5-318a7d23c16a/resourceGroups/reg-dev-qrgenerator/providers/Microsoft.ServiceBus/namespaces/sb-dev-qrgenerator')
resource sbdevqrgenerator 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  name: 'sb-${resourceName}'
  location: location
  tags: {}
  properties: {
    disableLocalAuth: false
    zoneRedundant: false
  }
}
@description('Generated from /subscriptions/74faf385-f368-4570-81a5-318a7d23c16a/resourceGroups/reg-dev-qrgenerator/providers/Microsoft.SignalRService/SignalR/sr-dev-qrgenerator')
resource srdevqrgenerator 'Microsoft.SignalRService/signalR@2022-02-01' = {
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

@description('Generated from /subscriptions/74faf385-f368-4570-81a5-318a7d23c16a/resourceGroups/reg-dev-qrgenerator/providers/Microsoft.Storage/storageAccounts/sadevqrgenerator')
resource sadevqrgenerator 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  name: 'sa${environment}${commonName}'
  location: location
  tags: {}
  properties: {
    defaultToOAuthAuthentication: false
    allowCrossTenantReplication: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      requireInfrastructureEncryption: false
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}
