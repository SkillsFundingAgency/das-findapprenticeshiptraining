using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseViewModelTests;

public sealed class WhenCreatingCourseViewModel
{
    [Test]
    public void GetLevelEquivalentToDisplayText_WithLevelsCountLessThanOne_ReturnsEmpty()
    {
        var sut = new CourseViewModel() { Level = 1 };
        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.Empty);
    }

    [Test]
    public void GetLevelEquivalentToDisplayText_WhenLevelNotFound_ReturnsEmpty()
    {
        var sut = new CourseViewModel()
        {
            Level = 1,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.Empty);
    }

    [Test]
    public void GetLevelEquivalentToDisplayText_WhenLevelFound_ReturnsEqualToLevelName()
    {
        var sut = new CourseViewModel()
        {
            Level = 2,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.GetLevelEquivalentToDisplayText(), Is.EqualTo("Equal to Level 2"));
    }

    [Test]
    public void GetTypicalJobTitles_WhenEmpty_ReturnsEmptyArray()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = string.Empty
        };

        Assert.That(sut.GetTypicalJobTitles(), Is.Empty);
    }

    [Test]
    public void GetTypicalJobTitles_WhenPopulated_ReturnsSplitArray()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = "Software Engineer|Construction Worker"
        };

        Assert.That(sut.GetTypicalJobTitles(), Is.EqualTo(["Software Engineer", "Construction Worker"]));
    }

    [Test]
    public void GetProviderCountDisplayMessage_WithLocationAndNoProviders_ReturnsZeroProviderWithinDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1"
        };

        var sut = model.GetProviderCountDisplayMessage();

        Assert.That(sut, Is.EqualTo(CourseViewModel.ZERO_PROVIDERS_WITHIN_DISTANCE_MESSAGE));
    }

    [Test]
    public void GetProviderCountDisplayMessage_WithLocationAndOneProvider_ReturnsSingleProviderWithinDistanceMessage()
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
    public void GetProviderCountDisplayMessage_WithLocationAndMultipleProviders_ReturnsMultipleProviderWithinDistanceMessage()
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
    public void GetProviderCountDisplayMessage_WithoutLocationAndOneProvider_ReturnsSingleProviderOutsideDistanceMessage()
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
    public void GetProviderCountDisplayMessage_WithoutLocationAndMultipleProviders_ReturnsMultipleProvidersOutsideDistanceMessage()
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
    public void GetApprenticeCanTravelDisplayMessage_WithDistanceAll_ReturnsAcrossEnglandDisplayText()
    {
        var sut = new CourseViewModel()
        {
            Distance = "All"
        };

        Assert.That(sut.GetApprenticeCanTravelDisplayMessage(), Is.EqualTo(DistanceService.ACROSS_ENGLAND_DISPLAY_TEXT));
    }

    [Test]
    public void GetApprenticeCanTravelDisplayMessage_WithDistanceSet_ReturnsMilesDisplayText()
    {
        var sut = new CourseViewModel()
        {
            Distance = "10"
        };

        Assert.That(sut.GetApprenticeCanTravelDisplayMessage(), Is.EqualTo($"{sut.Distance} miles"));
    }

    [Test, MoqAutoData]
    public void GetHelpFindingCourseUrl_WithLocationSet_ReturnsRequestApprenticeshipTrainingUrlWithLocation(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel() { LarsCode = "1", Location = "SW1" };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + "&location=SW1")}";

        var result = _sut.GetHelpFindingCourseUrl(findApprenticeshipTrainingWebConfiguration);
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void GetHelpFindingCourseUrl_WithoutLocationSet_ReturnsRequestApprenticeshipTrainingUrlWithoutLocation(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel() { LarsCode = "1", Location = string.Empty };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri)}";

        var result = _sut.GetHelpFindingCourseUrl(findApprenticeshipTrainingWebConfiguration);
        Assert.That(expectedLink, Is.EqualTo(result));
    }
}
