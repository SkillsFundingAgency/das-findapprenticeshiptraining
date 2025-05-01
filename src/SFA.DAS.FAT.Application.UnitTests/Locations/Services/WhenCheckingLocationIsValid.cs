using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Locations.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Locations.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Locations.Services;
public class WhenCheckingLocationIsValid
{
    [Test]
    [MoqInlineAutoData("", "", false, 0)]
    [MoqInlineAutoData("a", "", false, 0)]
    [MoqInlineAutoData("  a    ", "", false, 0)]
    [MoqInlineAutoData("ab", "", false, 0)]
    [MoqInlineAutoData("    ab   ", "", false, 0)]
    [MoqInlineAutoData("abc", "abc", true, 1)]
    [MoqInlineAutoData("   abc   ", "abc", true, 1)]
    [MoqInlineAutoData("abc, def", "abc", true, 1)]
    [MoqInlineAutoData("ghi jkl", "ghi", true, 1)]
    public async Task Then_Selected_Location_Is_Expected_Validity(
        string location,
        string locationUsedInApiCall,
        bool expectedValidity,
        int apiCalled,
        string baseUrl,
        Domain.Locations.Locations apiResponse,
        Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        LocationService service)
    {
        apiResponse.LocationItems = new List<Domain.Locations.Locations.LocationItem> { new() { Name = location } };

        //Arrange
        apiClient.Setup(x =>
                x.Get<Domain.Locations.Locations>(
                    It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(locationUsedInApiCall))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.IsLocationValid(location);

        //Assert
        actual.Should().Be(expectedValidity);

        apiClient.Verify(x => x.Get<Domain.Locations.Locations>(It.Is<GetLocationsApiRequest>(x => x.GetUrl.Contains(locationUsedInApiCall))), Times.Exactly(apiCalled));
    }

    [Test]
    [MoqInlineAutoData("", "", 0)]
    [MoqInlineAutoData("a", "", 0)]
    [MoqInlineAutoData("  a    ", "", 0)]
    [MoqInlineAutoData("ab", "", 0)]
    [MoqInlineAutoData("    ab   ", "", 0)]
    [MoqInlineAutoData("abc", "abc", 1)]
    [MoqInlineAutoData("   abc   ", "abc", 1)]
    [MoqInlineAutoData("abc, def", "abc", 1)]
    [MoqInlineAutoData("ghi jkl", "ghi", 1)]
    public async Task Then_Selected_Location_Returns_No_Details(
        string location,
        string locationUsedInApiCall,
        int apiCalled,
        string baseUrl,
        Domain.Locations.Locations apiResponse,
        Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        LocationService service)
    {
        apiResponse.LocationItems = new List<Domain.Locations.Locations.LocationItem> { new() { Name = location } };

        //Arrange
        apiClient.Setup(x =>
                x.Get<Domain.Locations.Locations>(
                    It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(locationUsedInApiCall))))
            .ReturnsAsync((Domain.Locations.Locations)null);

        //Act
        var actual = await service.IsLocationValid(location);

        //Assert
        actual.Should().Be(false);

        apiClient.Verify(x => x.Get<Domain.Locations.Locations>(It.IsAny<GetLocationsApiRequest>()), Times.Exactly(apiCalled));
    }
}
