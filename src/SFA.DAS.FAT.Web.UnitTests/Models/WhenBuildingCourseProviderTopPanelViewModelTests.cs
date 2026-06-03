using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenBuildingCourseProviderTopPanelViewModelTests
{
    [Test, MoqAutoData]
    public void Then_Set_ProviderRouteData(CourseProviderTopPanelViewModel sut)
    {

        var expectedDictionary = new Dictionary<string, string>
        {
            {"larsCode", sut.LarsCode},
            {"providerId", sut.Ukprn.ToString()}
        };

        sut.ProviderRouteData.Should().BeEquivalentTo(expectedDictionary);
    }
}
