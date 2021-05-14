# Demo

## Enabling AGIC add-on

### Azure CLI

  ```bash
  az aks enable-addons -n myCluster -g myResourceGroup -a ingress-appgw --appgw-id $appgwId
  ```

### Azure Bicep ( ARM )

  ```javascript
  ...
  addonProfiles: {
    ingressApplicationGateway: {
        enabled: false
        config: {
            ApplicationGatewayName: agicAppGwName
            SubnetPrefix: agicSubnetPrefix
            WatchNamespace: 'default'
        }
    }
  }
  ...
  ```

### Continuous configuration using Ingress controller objects

```yaml
- http:
    paths:
    - path: /
    backend:
        serviceName: aspnetapp
        servicePort: 80
    - path: /node
    backend:
        serviceName: node-hello
        servicePort: 80
```

### AGIC ingress annotations

- [Ingress for AKS annotations](https://github.com/Azure/application-gateway-kubernetes-ingress/blob/master/docs/annotations.md)

[<- Previous](incluster-addon.md)
