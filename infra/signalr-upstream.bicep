@description('Resource group')
param resourceGroup string

@description('Location')
param location string

@description('User-assigned identity')
param userAssignedIdentity string

@description('Hub Function App name')
param hubFunctionName string

@description('SignalR resource name')
param signalRName string

param utcValue string = utcNow()

resource runAzureCliScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'runAzureCliScript'
  location: location
  kind: 'AzureCLI'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentity}': {}
    }
  }
  properties: {
    forceUpdateTag: utcValue
    azCliVersion: '2.35.0'
    // scriptContent: loadTextContent('update-signalr-upstream.ps1')
    scriptContent: ''' 
    resourceGroup=$1
    hubFunctionName=$2
    signalRName=$3

    signalRKey=$(az functionapp keys list --name $hubFunctionName --resource-group $resourceGroup --query [systemKeys.signalr_extension] -o tsv);

    hubUrl="https://$hubFunctionName.azurewebsites.net/runtime/webhooks/signalr?code=$signalRKey"

    az signalr upstream update --name $signalRName --resource-group $resourceGroup --template url-template=$hubUrl
    '''
    arguments: '${resourceGroup} ${hubFunctionName} ${signalRName}'
    timeout: 'PT1H'
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
  }
}
