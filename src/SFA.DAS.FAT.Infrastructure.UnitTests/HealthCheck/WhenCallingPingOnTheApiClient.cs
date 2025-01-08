using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Api;
using SFA.DAS.FAT.Infrastructure.HealthCheck;
using SFA.DAS.FAT.Infrastructure.UnitTests.HttpMessageHandlerMock;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Infrastructure.UnitTests.HealthCheck
{
    public class WhenCallingPingOnTheApiClient
    {
        [Test, MoqAutoData]
        public async Task Then_The_Ping_Endpoint_Is_Called_For_OuterApi(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            FatOuterApiHealthCheck healthCheck)
        {
            //Act
            await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            client.Verify(x => x.Ping(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Endpoint_Is_Called_And_The_Status_is_Returned(
            FindApprenticeshipTrainingApi config)
        {
            //Arrange
            config.PingUrl = "https://test.local/";
            var configMock = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
            configMock.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, $"{config.PingUrl}ping", config.Key, HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            var apiClient = new ApiClient(client, configMock.Object);
            //Act
            var actual = await apiClient.Ping();
            //Assert
            actual.Should().Be((int)HttpStatusCode.Accepted);
        }

        [Test, MoqAutoData]
        public async Task Then_If_It_Is_Successful_200_Is_Returned(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            FatOuterApiHealthCheck healthCheck)
        {
            //Arrange
            client.Setup(x => x.Ping())
                .ReturnsAsync(200);
            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            actual.Status.Should().Be(HealthStatus.Healthy);
        }

        [Test, MoqAutoData]
        public async Task Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            FatOuterApiHealthCheck healthCheck)
        {
            //Arrange
            client.Setup(x => x.Ping())
                .ReturnsAsync(404);
            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            actual.Status.Should().Be(HealthStatus.Unhealthy);
        }
    }
}
