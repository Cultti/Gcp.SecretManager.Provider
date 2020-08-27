using Google.Cloud.SecretManager.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gcp.SecretManager.Provider.Helpers
{
    public class ServiceClientHelper
    {
        public virtual SecretManagerServiceClient Create()
            => SecretManagerServiceClient.Create();

        public virtual SecretManagerServiceClient Create(string credentialsPath)
        {
            var clientBuilder = new SecretManagerServiceClientBuilder()
            {
                CredentialsPath = credentialsPath
            };

            return clientBuilder.Build();
        }
            
    }
}
