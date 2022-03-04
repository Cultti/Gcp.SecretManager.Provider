using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.SecretManager.V1;
using Xunit;

namespace Gcp.SecretManager.Provider.Tests
{
    public class DefaultSecretManagerConfigurationLoaderTests
    {

        private readonly DefaultSecretManagerConfigurationLoader _target;
        public DefaultSecretManagerConfigurationLoaderTests()
        {
            _target = new DefaultSecretManagerConfigurationLoader();
        }

        [Fact]
        public void Load_ReturnsAlwaysTrue()
        {
            var result = _target.Load(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData("This__Is__MultiLevel", "This:Is:MultiLevel")]
        [InlineData("SingleLevel", "SingleLevel")]
        [InlineData("MultiLevel_With__One_UnderScore", "MultiLevel_With:One_UnderScore")]
        public void GetKey_ReplacesUnderscoresWithColon(string secret, string expected)
        {
            var result = _target.GetKey(new Secret
            {
                SecretName = new SecretName("SomeProjectId", secret)
            });

            Assert.Equal(expected, result);
        }
    }
}
