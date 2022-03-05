# [Google Secret Manager](https://cloud.google.com/secret-manager/) ConfigurationProvider
![.NET Core](https://github.com/Cultti/Gcp.SecretManager.Provider/workflows/.NET%20Core/badge.svg) [![codecov](https://codecov.io/gh/Cultti/Gcp.SecretManager.Provider/branch/master/graph/badge.svg)](https://codecov.io/gh/Cultti/Gcp.SecretManager.Provider)

Provides access to Google Secret Manager trough ConfigurationProvider

```
dotnet add package Gcp.SecretManager.Provider
```

## Before use
1. [Enable Secret Manager API from console](https://console.developers.google.com/apis/api/secretmanager.googleapis.com/overview)
2. [Create new Service Account from console](https://console.cloud.google.com/apis/credentials/serviceaccountkey)
   - It is recommended to give *Secret Manager Secret Accessor* -role
   - Save JSON key somewhere safe. We need it later.
3. [Add secrets to Secret Manager](https://console.cloud.google.com/security/secret-manager)

## How to use
1. Add this package trough package manager
```
dotnet add package Gcp.SecretManager.Provider
```
2. Configure secret manager as configuration source
```
config.AddGcpSecretManager(options => {
   options.ProjectId = "ProjectId"; // Required
   options.CredentialsPath = "/path/to/credentials"; // Optional
   options.Loader = new DefaultSecretManagerConfigurationLoader() // Optional, see more info below
});
```
*You can also provide CredentialsPath with GOOGLE_APPLICATION_CREDENTIALS environment variable*

3. Ready to go!

## Loaders
Loaders handles if secret should be loaded and mapping from Secret Manager keys to application configuration values by implementing contract `ISecretManagerConfigurationLoader`. This can be passed as an option during setup.

Contract exposes two method: `Load` and `GetKey`. `Load` method determines if the key should be loaded or not and `GetKey` handles mapping from secret to application configuration. You may access secret ID from `secret.SecretName.SecretId`

If no loader is specified then `DefaultSecretManagerConfigurationLoader` will be used. It loads all keys and hierarcy is added by adding two underscores in the secret name. Eg. `MultiLevel__Secret` maps to `MultiLevel:Secret` key in application configuration.