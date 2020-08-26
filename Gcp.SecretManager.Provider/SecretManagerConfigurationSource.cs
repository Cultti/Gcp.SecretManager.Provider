using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace Gcp.SecretManager.Provider
{
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        private readonly SecretManagerConfigurationOptions _options;

        public SecretManagerConfigurationSource(SecretManagerConfigurationOptions options)
        {
            _options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var client = CreateClient();
            var projectName = new ProjectName(_options.ProjectId);

            return new SecretManagerConfigurationProvider(client, projectName);
        }

        private SecretManagerServiceClient CreateClient()
        {
            SecretManagerServiceClient client;
            if (string.IsNullOrEmpty(_options.CredentialsPath))
            {
                client = SecretManagerServiceClient.Create();
            }
            else
            {
                var clientBuilder = new SecretManagerServiceClientBuilder()
                {
                    CredentialsPath = _options.CredentialsPath
                };

                client = clientBuilder.Build();
            }

            return client;
        }
    }
}
