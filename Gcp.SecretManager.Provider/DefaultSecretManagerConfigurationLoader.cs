using Gcp.SecretManager.Provider.Contracts;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace Gcp.SecretManager.Provider
{
    public class DefaultSecretManagerConfigurationLoader : ISecretManagerConfigurationLoader
    {
        public string GetKey(Secret secret)
            => secret.SecretName.SecretId.Replace("__", ConfigurationPath.KeyDelimiter);

        public bool Load(Secret secret)
            => true;
    }
}
