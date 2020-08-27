using Gcp.SecretManager.Provider.Helpers;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace Gcp.SecretManager.Provider.Tests
{
    public class SecretManagerConfigurationSourceTests
    {
        private readonly Mock<ServiceClientHelper> _mockServiceClientHelper;
        private readonly Mock<IConfigurationBuilder> _mockConfigurationBuilder;

        public SecretManagerConfigurationSourceTests()
        {
            _mockServiceClientHelper = new Mock<ServiceClientHelper>(MockBehavior.Strict);
            _mockConfigurationBuilder = new Mock<IConfigurationBuilder>(MockBehavior.Strict);
        }

        [Fact]
        public void Should_CreateProviderWithoutCredentialPath_When_CredentialsPathIsNull()
        {
            _mockServiceClientHelper.Setup(x => x.Create()).Returns<SecretManagerServiceClient>(null);
            var options = new SecretManagerConfigurationOptions
            {
                ProjectId = "ProjectId"
            };

            var configurationSource = new SecretManagerConfigurationSource(options, _mockServiceClientHelper.Object);
            var provider = configurationSource.Build(_mockConfigurationBuilder.Object);

            _mockServiceClientHelper.Verify(x => x.Create(), Times.Once);
        }

        [Fact]
        public void Should_CreateProviderWithCredentialPath_When_CredentialsPathIsNotNull()
        {
            string credentialPath = "/Test/Credential/Path";
            _mockServiceClientHelper.Setup(x => x.Create(credentialPath)).Returns<SecretManagerServiceClient>(null);
            var options = new SecretManagerConfigurationOptions
            {
                ProjectId = "ProjectId",
                CredentialsPath = credentialPath
            };

            var configurationSource = new SecretManagerConfigurationSource(options, _mockServiceClientHelper.Object);
            var provider = configurationSource.Build(_mockConfigurationBuilder.Object);

            _mockServiceClientHelper.Verify(x => x.Create(credentialPath), Times.Once);
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_ProjectIdIsNull()
        {
            var options = new SecretManagerConfigurationOptions();

            var configurationSource = new SecretManagerConfigurationSource(options, _mockServiceClientHelper.Object);
            Assert.Throws<ArgumentNullException>(() => configurationSource.Build(_mockConfigurationBuilder.Object));
        }
    }
}
