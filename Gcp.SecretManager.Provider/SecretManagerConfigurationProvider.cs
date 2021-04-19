using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Gcp.SecretManager.Provider
{
    public class SecretManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly SecretManagerServiceClient _client;
        private ProjectName _projectName;

        public SecretManagerConfigurationProvider(SecretManagerServiceClient client, ProjectName projectName)
        {
            _client = client;
            _projectName = projectName;
        }

        public override void Load()
        {
            LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task LoadAsync()
        {
            var secrets = _client.ListSecrets(_projectName);

            foreach (var secret in secrets)
            {
                try
                {
                    var secretVersionName = new SecretVersionName(secret.SecretName.ProjectId, secret.SecretName.SecretId, "latest");
                    var secretVersion = await _client.AccessSecretVersionAsync(secretVersionName);
                    Set(ConvertDelimiter(secret.SecretName.SecretId), secretVersion.Payload.Data.ToStringUtf8());
                } catch (Grpc.Core.RpcException)
                {
                    // This might happen if secret is created but it has no versions available
                    // For now just ignore. Maybe in future we should log that something went wrong?
                }
            }

        }

        private static string ConvertDelimiter(string key)
        {
            return key.Replace("__", ConfigurationPath.KeyDelimiter);
        }
    }
}
