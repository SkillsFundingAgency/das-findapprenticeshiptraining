using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests;
public class WhenLoadingTrainingOptionsViewModel
{
    [TestCase("", "", "", false)]
    [TestCase("10", "", "", false)]
    [TestCase("", "Coventry", "", false)]
    [TestCase("10", "Coventry", "within 10 miles", true)]
    [TestCase(DistanceService.ACROSS_ENGLAND_FILTER_VALUE, "Coventry", "", false)]
    public void Then_Filters_Must_Contain_Location_Filter_Section(string distance, string location, string expectedDistanceDetails, bool showDistanceDetails)
    {
        var sut = new TrainingOptionsViewModel
        {
            IsEmployerLocationAvailable = true,
            IsBlockReleaseAvailable = true,
            IsBlockReleaseMultiple = false,
            IsDayReleaseAvailable = true,
            IsDayReleaseMultiple = true,
            Distance = distance,
            Location = location
        };

        sut.DistanceDetails.Should().Be(expectedDistanceDetails);
        sut.ShowDistanceDetails.Should().Be(showDistanceDetails);
    }
}
