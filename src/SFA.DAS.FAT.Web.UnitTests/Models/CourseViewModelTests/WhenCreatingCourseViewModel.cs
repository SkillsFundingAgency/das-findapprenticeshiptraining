using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseViewModelTests;

public sealed class WhenCreatingCourseViewModel
{
    [Test, MoqAutoData]
    public void Then_The_Model_Is_Converted_From_Result_Correctly(GetCourseQueryResult source, ApprenticeshipType apprenticeshipType)
    {
        source.ApprenticeshipType = apprenticeshipType.ToString();
        var sut = (CourseViewModel)source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.StandardUId, Is.EqualTo(source.StandardUId));
            Assert.That(sut.IFateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
            Assert.That(sut.LarsCode, Is.EqualTo(source.LarsCode));
            Assert.That(sut.ProvidersCountWithinDistance, Is.EqualTo(source.ProvidersCountWithinDistance));
            Assert.That(sut.TotalProvidersCount, Is.EqualTo(source.TotalProvidersCount));
            Assert.That(sut.Title, Is.EqualTo(source.Title));
            Assert.That(sut.Level, Is.EqualTo(source.Level));
            Assert.That(sut.TitleAndLevel, Is.EqualTo($"{source.Title} (level {source.Level})"));
            Assert.That(sut.Version, Is.EqualTo(source.Version));
            Assert.That(sut.OverviewOfRole, Is.EqualTo(source.OverviewOfRole));
            Assert.That(sut.Route, Is.EqualTo(source.Route));
            Assert.That(sut.RouteCode, Is.EqualTo(source.RouteCode));
            Assert.That(sut.MaxFunding, Is.EqualTo(source.MaxFunding));
            Assert.That(sut.TypicalDuration, Is.EqualTo(source.TypicalDuration));
            Assert.That(sut.TypicalJobTitles, Is.EqualTo(source.TypicalJobTitles));
            Assert.That(sut.StandardPageUrl, Is.EqualTo(source.StandardPageUrl));
            // Assert.That(sut.Skills, Is.EqualTo(source.Skills));
            // Assert.That(sut.Knowledge, Is.EqualTo(source.Knowledge));
            // Assert.That(sut.Behaviours, Is.EqualTo(source.Behaviours));
            Assert.That(sut.Levels, Is.EqualTo(source.Levels));
            Assert.That(sut.CourseId, Is.EqualTo(source.LarsCode));
            Assert.That(sut.ShowShortListLink, Is.True);
            Assert.That(sut.ShowApprenticeTrainingCoursesCrumb, Is.True);
            Assert.That(sut.ApprenticeshipType.ToString(), Is.EqualTo(source.ApprenticeshipType));
        });
    }

    [Test]
    public void When_Levels_Count_Is_Less_Than_One_Then_Level_Equivalent_Display_Text_Returns_Empty()
    {
        var sut = new CourseViewModel() { Level = 1 };
        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.Empty);
    }

    [Test]
    public void When_Level_Is_Not_Found_Then_Level_Equivalent_Display_Text_Returns_Empty()
    {
        var sut = new CourseViewModel()
        {
            Level = 1,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.Empty);
    }

    [Test]
    public void When_Level_Is_Found_Then_Level_Equivalent_Display_Text_Returns_Equal_To_Level_Name()
    {
        var sut = new CourseViewModel()
        {
            Level = 2,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.EqualTo("Equal to Level 2"));
    }

    [Test]
    public void When_Typical_Job_Titles_Is_Empty_Then_Empty_Array_Is_Returned()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = string.Empty
        };

        Assert.That(sut.GetTypicalJobTitles(), Is.Empty);
    }

    [Test]
    public void When_Typical_Job_Titles_Is_Populated_Then_Split_Array_Is_Returned()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = "Software Engineer|Construction Worker"
        };

        Assert.That(sut.GetTypicalJobTitles(), Is.EqualTo(["Software Engineer", "Construction Worker"]));
    }

    [Test]
    public void When_Model_Has_Location_And_No_Providers_Then_Zero_Provider_Within_Distance_Message_Is_Returned()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1"
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.ZERO_PROVIDERS_WITHIN_DISTANCE_MESSAGE));
    }

    [Test]
    public void When_Model_Has_Location_And_One_Provider_Then_Single_Provider_Within_Distance_Message_Is_Returned()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1",
            ProvidersCountWithinDistance = 1
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.SINGLE_PROVIDER_WITHIN_DISTANCE_MESSAGE));
    }

    [Test]
    public void When_Model_Has_Location_And_Multiple_Providers_Then_Multiple_Provider_Within_Distance_Message_Is_Returned()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1",
            ProvidersCountWithinDistance = 2
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.MULTPLE_PROVIDERS_WITHIN_DISTANCE_MESSAGE.Replace("{{ProvidersCountWithinDistance}}", model.ProvidersCountWithinDistance.ToString())));
    }

    [Test]
    public void When_Model_Has_No_Location_And_One_Provider_Then_Single_Provider_Outside_Distance_Message_Is_Returned()
    {
        var model = new CourseViewModel()
        {
            Location = string.Empty,
            TotalProvidersCount = 1
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.SINGLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE));
    }

    [Test]
    public void When_Model_Has_No_Location_And_Multiple_Providers_Then_Multiple_Providers_Outside_Distance_Message_Is_Returned()
    {
        var model = new CourseViewModel()
        {
            Location = string.Empty,
            TotalProvidersCount = 2
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.MULTIPLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE.Replace("{{TotalProvidersCount}}", model.TotalProvidersCount.ToString())));
    }

    [Test]
    public void When_Distance_Is_All_Then_Across_England_Display_Text_Is_Returned()
    {
        var sut = new CourseViewModel()
        {
            Distance = "All"
        };

        Assert.That(sut.GetApprenticeCanTravelDisplayMessage(), Is.EqualTo(DistanceService.ACROSS_ENGLAND_DISPLAY_TEXT));
    }

    [Test]
    public void When_Distance_Is_Set_Then_Miles_Display_Text_Is_Returned()
    {
        var sut = new CourseViewModel()
        {
            Distance = "10"
        };

        Assert.That(sut.GetApprenticeCanTravelDisplayMessage(), Is.EqualTo($"{sut.Distance} miles"));
    }

    [Test, MoqAutoData]
    public void When_Location_Is_Set_Link_Returns_Request_Apprenticeship_Training_Url_Without_Location(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel() { LarsCode = 1, Location = "SW1" };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + "&location=SW1")}";

        var result = _sut.GetHelpFindingCourseUrl(findApprenticeshipTrainingWebConfiguration);
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void When_Location_Is_Not_Set_Link_Returns_Request_Apprenticeship_Training_Url_Without_Location(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel() { LarsCode = 1, Location = string.Empty };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri)}";

        var result = _sut.GetHelpFindingCourseUrl(findApprenticeshipTrainingWebConfiguration);
        Assert.That(expectedLink, Is.EqualTo(result));
    }
}
