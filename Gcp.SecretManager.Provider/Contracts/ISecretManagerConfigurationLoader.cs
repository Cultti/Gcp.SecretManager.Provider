using Google.Cloud.SecretManager.V1;

namespace Gcp.SecretManager.Provider.Contracts
{
    public interface ISecretManagerConfigurationLoader
    {
        string GetKey(Secret secret);
        bool Load(Secret secret);
    }
}
