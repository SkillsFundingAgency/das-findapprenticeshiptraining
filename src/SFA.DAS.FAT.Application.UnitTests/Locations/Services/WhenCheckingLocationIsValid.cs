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
    [MoqInlineAutoData("c, oventry", "", false, 0)]
    [MoqInlineAutoData("co, ventry", "", false, 0)]
    [MoqInlineAutoData(" co, ventry", "", false, 0)]
    [MoqInlineAutoData("co , ventry", "", false, 0)]
    [MoqInlineAutoData("c oventry", "", true, 1)]
    public async Task Then_Selected_Location_Is_Expected_Validity(
        string location,
        string locationUsedInApiCall,
        bool expectedValidity,
        int apiCalled,
        string baseUrl,
        Domain.Locations apiResponse,
        Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        LocationService service)
    {
        apiResponse.LocationItems = new List<Domain.Locations.LocationItem> { new() { Name = location } };

        //Arrange
        apiClient.Setup(x =>
                x.Get<Domain.Locations>(
                    It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(locationUsedInApiCall))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.IsLocationValid(location);

        //Assert
        actual.Should().Be(expectedValidity);

        apiClient.Verify(x => x.Get<Domain.Locations>(It.Is<GetLocationsApiRequest>(x => x.GetUrl.Contains(locationUsedInApiCall))), Times.Exactly(apiCalled));
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
    [MoqInlineAutoData("ghi jkl", "ghi", 2)]
    public async Task Then_Location_Returns_No_Details_And_Calls_Api_When_Length_Above_2(
        string location,
        string locationUsedInApiCall,
        int apiCalled,
        string baseUrl,
        Domain.Locations apiResponse,
        Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        LocationService service)
    {
        apiResponse.LocationItems = new List<Domain.Locations.LocationItem> { new() { Name = location } };

        //Arrange
        apiClient.Setup(x =>
                x.Get<Domain.Locations>(
                    It.Is<GetLocationsApiRequest>(c => c.GetUrl.Contains(locationUsedInApiCall))))
            .ReturnsAsync(new Domain.Locations { LocationItems = new List<Domain.Locations.LocationItem>() });

        //Act
        var actual = await service.IsLocationValid(location);

        //Assert
        actual.Should().Be(false);

        apiClient.Verify(x => x.Get<Domain.Locations>(It.IsAny<GetLocationsApiRequest>()), Times.Exactly(apiCalled));
    }
}
