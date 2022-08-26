
param containerAppName  string
param location          string
param logsName          string
param envName           string
param imageName         string
param imageTag          string
param appEnvVars        object

module containerEnv 'ace.bicep' = {
  name: 'ace-deployment'
  params: {
    containerEnvName: '${containerAppName}-ace'
    location:         location
    logsName:         logsName
  }
}

module containerApp 'aca.bicep' = {
  name: 'aca-deployment'
  params: {
    env: envName
    containerEnvId: containerEnv.outputs.containerAppEnvID
    location: location
    containerAppName: containerAppName
    imageName: imageName
    imageTag: imageTag
    appEnvVars: appEnvVars
  }
}

// output c1 string = containerApp.outputs.acaId
// output c2 string = containerApp.outputs.acaSuffix
// output c3 object = containerApp.outputs.acaOut
