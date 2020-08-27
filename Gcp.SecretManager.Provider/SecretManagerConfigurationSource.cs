using Gcp.SecretManager.Provider.Helpers;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;
using System;

namespace Gcp.SecretManager.Provider
{
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        private readonly SecretManagerConfigurationOptions _options;
        private readonly ServiceClientHelper _clientHelper;

        public SecretManagerConfigurationSource(SecretManagerConfigurationOptions options, ServiceClientHelper clientHelper = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
            _clientHelper = clientHelper ?? new ServiceClientHelper();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (string.IsNullOrEmpty(_options.ProjectId))
            {
                throw new ArgumentNullException(nameof(_options.ProjectId));
            }

            var projectName = new ProjectName(_options.ProjectId);
            var client = CreateClient();

            return new SecretManagerConfigurationProvider(client, projectName);
        }

        private SecretManagerServiceClient CreateClient()
        {
            if (string.IsNullOrEmpty(_options.CredentialsPath))
            {
                return _clientHelper.Create();
            }
            else
            {
                return _clientHelper.Create(_options.CredentialsPath);
            }
        }
    }
}
