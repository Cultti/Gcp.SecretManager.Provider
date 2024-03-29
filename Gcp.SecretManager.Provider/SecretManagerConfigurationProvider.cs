﻿using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Gcp.SecretManager.Provider.Contracts;

namespace Gcp.SecretManager.Provider
{
    public class SecretManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly SecretManagerServiceClient _client;
        private readonly ProjectName _projectName;
        private readonly ISecretManagerConfigurationLoader _loader;

        public SecretManagerConfigurationProvider(
            SecretManagerServiceClient client,
            ProjectName projectName,
            ISecretManagerConfigurationLoader loader)
        {
            _client = client;
            _projectName = projectName;
            _loader = loader;
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
                    if (!_loader.Load(secret))
                    {
                        continue;
                    }

                    var secretVersionName = new SecretVersionName(secret.SecretName.ProjectId,
                        secret.SecretName.SecretId, "latest");
                    var secretVersion = await _client.AccessSecretVersionAsync(secretVersionName);
                    Set(_loader.GetKey(secret), secretVersion.Payload.Data.ToStringUtf8());
                }
                catch (Grpc.Core.RpcException)
                {
                    // This might happen if secret is created but it has no versions available
                    // For now just ignore. Maybe in future we should log that something went wrong?
                }
            }
        }
    }
}
