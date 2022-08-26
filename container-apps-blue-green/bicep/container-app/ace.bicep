
param logsName            string
param location            string
param containerEnvName    string

// var aceName =

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    } 
  }
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name:      containerEnvName
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

output containerAppEnvID string = containerAppEnv.id
