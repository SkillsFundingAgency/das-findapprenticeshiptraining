using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenBuildingCourseProviderTopPanelViewModelTests
{
    [Test]
    [MoqInlineAutoData("10", "10")]
    [MoqInlineAutoData("20", "20")]
    [MoqInlineAutoData("50", "50")]
    [MoqInlineAutoData(DistanceService.ACROSS_ENGLAND_FILTER_VALUE, "")]
    public void Then_Set_ProviderRouteData(string distance, string distanceExpected, CourseProviderTopPanelViewModel sut)
    {

        sut.Distance = distance;

        var expectedDictionary = new Dictionary<string, string>
        {
            {"location", sut.Location},
            {"larsCode", sut.LarsCode},
            {"providerId", sut.Ukprn.ToString()},
            {"distance", distanceExpected}
        };

        sut.ProviderRouteData.Should().BeEquivalentTo(expectedDictionary);
    }
}
