using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace Gcp.SecretManager.Provider.Tests
{
    public class SecretManagerConfigurationProviderExtensionsTests
    {
        private readonly Mock<IConfigurationBuilder> _mockConfigurationProvider;

        public SecretManagerConfigurationProviderExtensionsTests()
        {
            _mockConfigurationProvider = new Mock<IConfigurationBuilder>(MockBehavior.Strict);
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_OptionsMethodNotProvided()
        {
            Assert.Throws<ArgumentNullException>(() => _mockConfigurationProvider.Object.AddGcpSecretManager(null));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_ProjectIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mockConfigurationProvider.Object.AddGcpSecretManager((options) =>
            {

            }));
        }

        [Fact]
        public void Should_AddSecretManagerConfigurationSource_When_Configured()
        {
            _mockConfigurationProvider.Setup(x => x.Add(It.IsAny<SecretManagerConfigurationSource>())).Returns<IConfigurationBuilder>(null);

            _mockConfigurationProvider.Object.AddGcpSecretManager((options) =>
            {
                options.ProjectId = "TestId";
            });

            _mockConfigurationProvider.Verify(x => x.Add(It.IsAny<SecretManagerConfigurationSource>()), Times.Once);
        }
    }
}
