

param location            string
param containerAppName    string
param env                 string
param containerEnvId      string
param appEnvVars          object

param revSuffix           string = 'staging'
param cpuCore             string = '0.5'
param memorySize          string = '1'

param appName             string = 'default-app-name'

param imageName           string = 'default-image'
param imageTag            string = '1.0'

var registryName = 'ghcr.io/rvlabsmsft'
var containerImage    = '${registryName}/${imageName}:${imageTag}'

var productionSlot = {
  weight: 100
  label: 'production'
  revisionName: '${containerAppName}--production-${appEnvVars.appBuildId}'
}

var stagingSlot = {
  weight: 0
  label: 'staging'
  revisionName: '${containerAppName}--staging-${appEnvVars.appBuildId}'
}

resource currentAca 'Microsoft.App/containerApps@2022-03-01' existing = {
  name: containerAppName
}

resource aca 'Microsoft.App/containerApps@2022-03-01' = {
  name:     containerAppName
  location: location
  properties: {
    managedEnvironmentId: containerEnvId
    configuration: {
      activeRevisionsMode: 'Multiple'
      ingress: {
        external: true
        targetPort: 80
        allowInsecure:  false
        traffic: [
          productionSlot
          stagingSlot
        ]
      }
    }
    template: {
      revisionSuffix: '${revSuffix}-${appEnvVars.appBuildId}'
      containers: [
        {
          name: appName
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

// output acaOut object = currentAca.properties.configuration.ingress.traffic[0]
// output acaSuffix string = currentAca.properties.template.revisionSuffix
// output acaId string = currentAca.id
