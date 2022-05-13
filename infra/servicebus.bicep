@description('Service Bus name')
param name string

@description('Service Bus location')
param location string

@description('Function App runtime')
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param sku string

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  sku: {
    name: sku
    tier: sku
  }
  name: name
  location: location
  tags: {}
  properties: {
    disableLocalAuth: false
    zoneRedundant: false
  }
}

var sendKeyName = 'ApiSend'

resource serviceBusSendKey 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2021-11-01' = {
  name: '${serviceBus.name}/${sendKeyName}'
  properties: {
    rights: [
      'Send'
    ]
  }
}

var listKeysEndpoint = '${serviceBus.id}/AuthorizationRules/${sendKeyName}'
output sendConnectionString string = listKeys(listKeysEndpoint, serviceBus.apiVersion).primaryConnectionString
