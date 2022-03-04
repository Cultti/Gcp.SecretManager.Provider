using Gcp.SecretManager.Provider.Contracts;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace Gcp.SecretManager.Provider
{
    public class DefaultSecretManagerConfigurationLoader : ISecretManagerConfigurationLoader
    {
        public virtual string GetKey(Secret secret)
            => secret.SecretName.SecretId.Replace("__", ConfigurationPath.KeyDelimiter);

        public virtual bool Load(Secret secret)
            => true;
    }
}
