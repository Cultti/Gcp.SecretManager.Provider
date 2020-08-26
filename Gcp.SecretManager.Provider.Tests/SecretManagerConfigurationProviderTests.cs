using Gcp.SecretManager.Provider.Tests.Helpers;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gcp.SecretManager.Provider.Tests
{
    public class SecretManagerConfigurationProviderTests
    {
        private readonly Mock<SecretManagerServiceClient> _mockClient;
        private readonly List<Secret> _testSecrets;
        private readonly PagedEnumerableHelper<ListSecretsResponse, Secret> _pagedResponse;
        private const string _projectName = "TestProjectName";
        private const string _secretProjectName = "TestSecretProjectName";

        public SecretManagerConfigurationProviderTests()
        {
            _testSecrets = new List<Secret>
            {
                new Secret
                {
                    SecretName = new SecretName(_secretProjectName, "SecretId1")
                },
                new Secret
                {
                    SecretName = new SecretName(_secretProjectName, "SecretId2")
                },
                new Secret
                {
                    SecretName = new SecretName(_secretProjectName, "SecretId3")
                },
                new Secret
                {
                    SecretName = new SecretName(_secretProjectName, "SecretId4")
                }
            };
            _pagedResponse = new PagedEnumerableHelper<ListSecretsResponse, Secret>(_testSecrets);

            _mockClient = new Mock<SecretManagerServiceClient>(MockBehavior.Strict);
            _mockClient.Setup(x => x.ListSecrets(It.Is<ProjectName>(pn => pn.ProjectId == _projectName), null, null, null))
                .Returns(_pagedResponse);

            foreach (var secret in _testSecrets)
            {
                var response = new AccessSecretVersionResponse
                {
                    Payload = new SecretPayload
                    {
                        Data = Google.Protobuf.ByteString.CopyFromUtf8($"{secret.SecretName.SecretId}-Value")
                    }
                };
                _mockClient.Setup(
                    x => x.AccessSecretVersionAsync(
                        It.Is<SecretVersionName>(svn => svn.ProjectId == secret.SecretName.ProjectId &&
                                svn.SecretId == secret.SecretName.SecretId &&
                                svn.SecretVersionId == "latest"), null))
                    .ReturnsAsync(response);
            }
        }

        [Fact]
        public void Should_FetchSecrets_When_LoadIsCalled()
        {
            var configurationProvider = new SecretManagerConfigurationProvider(_mockClient.Object, new ProjectName(_projectName));
            configurationProvider.Load();

            foreach (var secret in _testSecrets)
            {
                configurationProvider.TryGet(secret.SecretName.SecretId, out string value);
                Assert.Equal($"{secret.SecretName.SecretId}-Value", value);
            }
        }

        [Fact]
        public void Should_FetchSecrets_When_LoadIsCalled_And_ExceptionIsThrown()
        {
            var errorSecretId = "ErrorSecret";
            _testSecrets.Add(new Secret
            {
                SecretName = new SecretName(_secretProjectName, errorSecretId)
            });

            _mockClient.Setup(
                    x => x.AccessSecretVersionAsync(
                        It.Is<SecretVersionName>(svn => svn.ProjectId == _secretProjectName &&
                                svn.SecretId == errorSecretId &&
                                svn.SecretVersionId == "latest"), null))
                    .ThrowsAsync(new Grpc.Core.RpcException(Grpc.Core.Status.DefaultCancelled));

            var configurationProvider = new SecretManagerConfigurationProvider(_mockClient.Object, new ProjectName(_projectName));
            configurationProvider.Load();

            foreach (var secret in _testSecrets.Where(x => x.SecretName.SecretId != errorSecretId))
            {
                configurationProvider.TryGet(secret.SecretName.SecretId, out string value);
                Assert.Equal($"{secret.SecretName.SecretId}-Value", value);
            }
        }
    }
}
