

param dnsPrefix string = 'coro-aks'
param agentVMSize string = 'Standard_D2_v3'

targetScope = 'resourceGroup'

var coreRgName = 'MAI-RvLabsCoreRG'
var coreVnetName = 'vnet-RvLCoreHub'

//vars
var clusterName = 'coro-cl01'
var kubernetesVersion = '1.19.0'
var subnetRef = resourceId(coreRgName, 'Microsoft.Network/virtualNetworks', virtualNetworkName)
var addressPrefix = '10.22.0.0/16'
var subnetName = 'coro-aks-snet'
var subnetPrefix = '10.22.100.0/24'
var virtualNetworkName = 'vnet-RvLCoreHub'
var nodeResourceGroup = '${dnsPrefix}-${clusterName}RG'
var tags = {
  environment: 'lab'
  projectCode: 'rvlabs'
}
var basePoolName = 'coro-NodePool'

// Azure kubernetes service
resource aks 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: clusterName
  location: resourceGroup().location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enableRBAC: true
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: 'systempool01'
        count: 1
        mode: 'System'
        vmSize: agentVMSize
        type: 'VirtualMachineScaleSets'
        osType: 'Linux'
        enableAutoScaling: false
        vnetSubnetID: '${subnetRef}/subnets/${subnetName}'
      }
      {
        name: 'workerpool01'
        count: 1
        mode: 'User'
        vmSize: agentVMSize
        type: 'VirtualMachineScaleSets'
        osType: 'Linux'
        enableAutoScaling: false
        vnetSubnetID: '${subnetRef}/subnets/${subnetName}'
      }
    ]
    servicePrincipalProfile: {
      clientId: 'msi'
    }
    nodeResourceGroup: nodeResourceGroup
    networkProfile: {
      networkPlugin: 'azure'
      loadBalancerSku: 'standard'
    }
    addonProfiles: {
      ingressApplicationGateway: {
        enabled: true
        config: {
          ApplicationGatewayName: 'coro-agic'
          SubnetPrefix: '10.22.115.0/24'
          WatchNamespace: 'default'
        }
      }
    }
  }
}

output id string = aks.id
output apiServerAddress string = aks.properties.fqdn
