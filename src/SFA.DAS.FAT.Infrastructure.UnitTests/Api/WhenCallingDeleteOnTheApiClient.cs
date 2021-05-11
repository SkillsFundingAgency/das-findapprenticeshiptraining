using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Api;
using SFA.DAS.FAT.Infrastructure.UnitTests.HttpMessageHandlerMock;

namespace SFA.DAS.FAT.Infrastructure.UnitTests.Api
{
    public class WhenCallingDeleteOnTheApiClient
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called(
            int id,
            FindApprenticeshipTrainingApi config)
        {
            //Arrange
            var configMock = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
            configMock.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted
            };
            var deleteTestRequest = new DeleteTestRequest(id,"https://test.local");
            var expectedUrl = deleteTestRequest.DeleteUrl;
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, config.Key, HttpMethod.Delete);
            var client = new HttpClient(httpMessageHandler.Object);
            var apiClient = new ApiClient(client, configMock.Object);
            
            
            //Act
            await apiClient.Delete(deleteTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Delete)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
        
        [Test, AutoData]
        public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown(
            int id,
            FindApprenticeshipTrainingApi config)
        {
            //Arrange
            var configMock = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
            configMock.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            var deleteTestRequest = new DeleteTestRequest(id,"https://test.local");
            var expectedUrl = deleteTestRequest.DeleteUrl;
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, config.Key, HttpMethod.Delete);
            var client = new HttpClient(httpMessageHandler.Object);
            var apiClient = new ApiClient(client, configMock.Object);
            
            //Act Assert
            Assert.ThrowsAsync<HttpRequestException>(() => apiClient.Delete(deleteTestRequest));
            
        }
        
        private class DeleteTestRequest : IDeleteApiRequest
        {
            private readonly int _id;

            public DeleteTestRequest (int id, string baseUrl)
            {
                _id = id;
                BaseUrl = baseUrl;
            }
            public string DeleteUrl => $"{BaseUrl}/test-url/delete/{_id}";
            public string BaseUrl { get; }
        }
    }
}