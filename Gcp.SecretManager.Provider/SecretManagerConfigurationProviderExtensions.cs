using Microsoft.Extensions.Configuration;
using System;

namespace Gcp.SecretManager.Provider
{
    public static class SecretManagerConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddGcpSecretManager(this IConfigurationBuilder configuration, Action<SecretManagerConfigurationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var configurationOptions = new SecretManagerConfigurationOptions();
            options(configurationOptions);

            if (string.IsNullOrEmpty(configurationOptions.ProjectId))
            {
                throw new ArgumentNullException(nameof(configurationOptions.ProjectId));
            }

            configuration.Add(new SecretManagerConfigurationSource(configurationOptions));

            return configuration;
        }
    }
}
