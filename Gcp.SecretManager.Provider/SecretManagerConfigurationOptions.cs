using Gcp.SecretManager.Provider.Contracts;

namespace Gcp.SecretManager.Provider
{
    public class SecretManagerConfigurationOptions
    { 
        public string CredentialsPath { get; set; }
        public string ProjectId { get; set; }
        public ISecretManagerConfigurationLoader Loader { get; set; }
    }
}
