@description('Storage Account name')
@minLength(3)
@maxLength(24)
param name string

@description('Storage Account location')
param location string

@description('Generated from /subscriptions/74faf385-f368-4570-81a5-318a7d23c16a/resourceGroups/reg-dev-qrgenerator/providers/Microsoft.Storage/storageAccounts/sadevqrgenerator')
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  name: name
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


var accountName = storageAccount.name
var key = listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${accountName};EndpointSuffix=core.windows.net;AccountKey=${key}'
