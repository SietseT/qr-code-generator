@description('Web App name')
param name string

@description('FWeb App location')
param location string

@description('App Service Plan Id')
param planId string

@description('Connection strings')
@secure()
param connectionStrings object

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  properties: {
    serverFarmId: planId
    siteConfig: {
      alwaysOn: false
      linuxFxVersion: 'DOTNETCORE|6.0'
      connectionStrings: connectionStrings
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Test'
        }
      ]
    }
  }
}
