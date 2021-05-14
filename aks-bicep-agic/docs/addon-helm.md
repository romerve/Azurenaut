# Github (Helm) deployment vs. add-on

- Since AGIC add-on is a managed service, customers will automatically be updated to the latest version of AGIC add-on
- Collect telemetry from AGIC add-on to improve supportability
- Helm deployment values cannot be modified on the AKS add-on:
  - **verbosityLevel** will be set to 5 by default
  - **usePrivateIp** will be set to be false by default; this can be overwritten by the use-private-ip annotation
  - **shared** is not supported on add-on
  - **reconcilePeriodSeconds** is not supported on add-on

- AGIC deployed via Helm supports **ProhibitedTargets**
  - AGIC with ProhibitedTargets can configure the Application Gateway to support AKS clusters without affecting other existing backends.
  - AGIC add-on doesn't currently support this.

[<- Previous](agic-overview.md) - [Next ->](incluster-addon.md)