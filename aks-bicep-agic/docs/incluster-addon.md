# Differences between In-Cluster & Add-On

- Add-on is fully managed service, supported by AKS + AppGW

- Reduces latency by eliminating an extra hop

- Eliminates the noisy neighbor problem (AGIC = only 1 pod)

- Easily use built in WAF on AppGW

- Leverage autoscaling on AppGW

- Limited feature set compared to some other in-cluster options

[<- Previous](addon-helm.md) - [Next ->](agic-demo.md)