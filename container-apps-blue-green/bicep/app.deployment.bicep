

param containerAppName  string

@description('Container image including repository and tag')
param containerImage    string

param location          string = resourceGroup().location

param cpuCore           string = '0.5'
param memorySize        string = '1'

param appEnvVars object = {
  appCommit: '##commit##'
  appBuildId: '##buildid##'
  appSlot: ''
}

param label string

param app object = {
  //label: 'dev'
  weight: 100
  revisionName: '${containerAppName}--${appEnvVars.appBuildId}'
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'logs-${containerAppName}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    } 
  }
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2022-10-01' = {
  name:      'env-${containerAppName}'
  location:  location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId:   logAnalytics.properties.customerId
        sharedKey:    logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource containerApp 'Microsoft.App/containerApps@2022-10-01' = {
  name:     containerAppName
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
      activeRevisionsMode: 'Multiple'
    }
    template: {
      revisionSuffix: '${label}-${appEnvVars.appBuildId}'
      containers: [
        {
          name: containerAppName
          image: containerImage
          resources: {
            cpu: json(cpuCore)
            memory: '${memorySize}Gi'
          }
          env: [
            {
              name: 'APPCOMMIT'
              value: appEnvVars.appCommit
            }
            {
              name: 'APPBUILDID'
              value: appEnvVars.appBuildId
            }
            {
              name: 'APPSLOT'
              value: appEnvVars.appSlot
            }
          ]
        }
      ]
    }
  }
}
