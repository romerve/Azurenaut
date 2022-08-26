targetScope = 'subscription'

@description('Container App name')
param containerAppName string 

@description('App environment: Default is staging')
@allowed([
  'staging'
  'production'
])
param environment string = 'staging'

@description('Deployment region: defaults to EastUS')
param location string = 'eastus'

param appEnvVars object = {
  appCommit: '##commit##'
  appBuildId: '##buildid##'
  appSlot: environment
}

param imageName string
param imageTag string

var acaRgName = 'aca-${containerAppName}-rg'
var logsName = 'logs-${containerAppName}'


resource containerAppsRG 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: acaRgName
  location: location
}

module containerAppService 'container-app/main.containerapps.bicep' = {
  scope: containerAppsRG
  name: 'ACA-bicep-deployment'
  params: {
    containerAppName: containerAppName
    envName: environment
    logsName: logsName
    location: location
    imageName: imageName
    imageTag: imageTag
    appEnvVars: appEnvVars
  }
}

// output a1 string = containerAppService.outputs.c1
// output a2 string = containerAppService.outputs.c2
// output a3 object = containerAppService.outputs.c3
