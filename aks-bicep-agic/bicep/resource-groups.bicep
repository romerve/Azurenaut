
param rgName string = 'coro-aks-RG'
param location string = 'eastus'

targetScope = 'subscription'

resource aksRg 'Microsoft.Resources/resourceGroups@2020-10-01' = {
  name: rgName
  location: location
  tags: {}
  properties: {
  }
}

module aksResource 'aks-resource.bicep' = {
  name: 'AKS-deployment'
  scope: aksRg
}

output id string = aksResource.outputs.id
output apiServerAddress string = aksResource.outputs.apiServerAddress
