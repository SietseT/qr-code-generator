@description('Web App name')
param name string

@description('Web App location')
param location string

@description('App Service Plan Id')
param planId string

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  properties: {
    serverFarmId: planId
    siteConfig: {
      alwaysOn: false
      netFrameworkVersion: 'v6.0'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Test'
        }
      ]
    }
    httpsOnly: true
  }
}
