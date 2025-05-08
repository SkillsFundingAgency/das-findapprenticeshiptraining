using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Locations.Services;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Locations.Services
{
    public class WhenGettingLocationsByQuery
    {
        [Test, MoqAutoData]
        public async Task Then_The_Data_Is_Returned_From_The_Api(
            string searchTerm,
            string baseUrl,
            Domain.Locations apiResponse,
            Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            LocationService service)
        {
            //Arrange
            apiClient.Setup(x =>
                    x.Get<Domain.Locations>(
                        It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(searchTerm))))
                .ReturnsAsync(apiResponse);

            //Act
            var actual = await service.GetLocations(searchTerm);

            //Assert
            actual.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        [MoqInlineAutoData("")]
        [MoqInlineAutoData("a")]
        [MoqInlineAutoData("ab")]
        [MoqInlineAutoData(" ab")]
        [MoqInlineAutoData("ab ")]
        [MoqInlineAutoData(" ab ")]
        public async Task Then_Trimmed_Search_Term_Less_Than_3_Should_Not_Invoke_Api_Call(
            string searchTerm,
            string baseUrl,
            Domain.Locations apiResponse,
            Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            LocationService service)
        {
            //Arrange
            apiClient.Setup(x =>
                    x.Get<Domain.Locations>(
                        It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(searchTerm))))
                .ReturnsAsync(apiResponse);

            //Act
            var actual = await service.GetLocations(searchTerm);

            //Assert
            actual.Should().BeEquivalentTo(new Domain.Locations());

            apiClient.Verify(a => a.Get<Domain.Locations>(It.IsAny<GetLocationsApiRequest>()), Times.Never);
        }
    }
}
