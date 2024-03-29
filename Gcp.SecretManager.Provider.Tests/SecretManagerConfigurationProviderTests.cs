using Gcp.SecretManager.Provider.Tests.Helpers;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Gcp.SecretManager.Provider.Contracts;
using Newtonsoft.Json.Bson;
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

        private readonly SecretManagerConfigurationProvider _target;

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

            _target = new SecretManagerConfigurationProvider(_mockClient.Object, new ProjectName(_projectName), new DefaultSecretManagerConfigurationLoader());
        }

        [Fact]
        public void Should_FetchSecrets_When_LoadIsCalled()
        {
            _target.Load();

            foreach (var secret in _testSecrets)
            {
                _target.TryGet(secret.SecretName.SecretId, out string value);
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

            _target.Load();

            foreach (var secret in _testSecrets.Where(x => x.SecretName.SecretId != errorSecretId))
            {
                _target.TryGet(secret.SecretName.SecretId, out string value);
                Assert.Equal($"{secret.SecretName.SecretId}-Value", value);
            }
        }

        [Fact]
        public void Should_FetcHierarchicalSecrets_When_LoadIsCalled()
        {
            var googleName = "Multi__Level__Secret";
            var dotNetName = "Multi:Level:Secret";
            var value = "SecretValue";

            var response = new AccessSecretVersionResponse
            {
                Payload = new SecretPayload
                {
                    Data = Google.Protobuf.ByteString.CopyFromUtf8(value)
                }
            };
            _mockClient.Setup(
                x => x.AccessSecretVersionAsync(
                    It.Is<SecretVersionName>(svn => svn.ProjectId == _secretProjectName &&
                            svn.SecretId == googleName &&
                            svn.SecretVersionId == "latest"), null))
                .ReturnsAsync(response);

            _testSecrets.Add(new Secret
            {
                SecretName = new SecretName(_secretProjectName, googleName),
            });
            var pagedResponse = new PagedEnumerableHelper<ListSecretsResponse, Secret>(_testSecrets);
            _mockClient.Setup(x => x.ListSecrets(It.Is<ProjectName>(pn => pn.ProjectId == _projectName), null, null, null))
                .Returns(pagedResponse);


            _target.Load();

            Assert.True(_target.TryGet(dotNetName, out var configValue));
            Assert.Equal(value, configValue);
        }

        [Fact]
        public void Should_CallLoaderGetKey()
        {
            var mockLoader = new Mock<ISecretManagerConfigurationLoader>();
            mockLoader.Setup(m => m.GetKey(It.IsAny<Secret>()))
                .Returns((Secret secret) => secret.SecretName.SecretId);
            mockLoader.Setup(m => m.Load(It.IsAny<Secret>())).Returns(true);

            var target = new SecretManagerConfigurationProvider(_mockClient.Object, new ProjectName(_projectName), mockLoader.Object);

            target.Load();

            mockLoader.Verify(m => m.GetKey(It.IsAny<Secret>()), Times.Exactly(_testSecrets.Count()));
        }

        [Fact]
        public void Should_NotLoadAllSecrets()
        {
            var mockLoader = new Mock<ISecretManagerConfigurationLoader>();
            mockLoader.Setup(m => m.GetKey(It.IsAny<Secret>()))
                .Returns((Secret secret) => secret.SecretName.SecretId);
            mockLoader.Setup(m => m.Load(It.IsAny<Secret>())).Returns((Secret secret) =>
                secret.SecretName.SecretId != _testSecrets.First().SecretName.SecretId);

            var target = new SecretManagerConfigurationProvider(_mockClient.Object, new ProjectName(_projectName), mockLoader.Object);

            target.Load();

            Assert.False(target.TryGet("SecretId1", out var configValue1));
            Assert.True(target.TryGet("SecretId2", out var configValue2));
            Assert.True(target.TryGet("SecretId3", out var configValue3));
            Assert.True(target.TryGet("SecretId4", out var configValue4));
        }
    }
}
