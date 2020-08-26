# [Google Secret Manager](https://cloud.google.com/secret-manager/) ConfigurationProvider
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
});
```
*You can also provide CredentialsPath with GOOGLE_APPLICATION_CREDENTIALS environment variable*

3. Ready to go!