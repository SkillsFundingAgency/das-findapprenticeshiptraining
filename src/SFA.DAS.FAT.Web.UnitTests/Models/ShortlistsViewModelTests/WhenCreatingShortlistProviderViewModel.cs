using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistsViewModelTests;

public class WhenCreatingShortlistProviderViewModel
{
    [TestCase(true, true, true, 3)]
    [TestCase(true, false, true, 2)]
    [TestCase(true, true, false, 2)]
    [TestCase(true, false, false, 1)]
    [TestCase(false, true, false, 1)]
    [TestCase(false, false, true, 1)]
    public void ThenNoOfDeliveryOptionsIsAsExpected(bool hasBlockRelease, bool hasDayRelease, bool atEmployer, int expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            AtEmployer = atEmployer,
            HasBlockRelease = hasBlockRelease,
            HasDayRelease = hasDayRelease
        };
        sut.NoOfDeliveryOptions().Should().Be(expected);
    }

    [TestCase(true, true, true, true)]
    [TestCase(true, false, true, true)]
    [TestCase(true, true, false, true)]
    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    public void ThenHasMultipleDeliveryOptionsIsAsExpected(bool hasBlockRelease, bool hasDayRelease, bool atEmployer, bool expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            AtEmployer = atEmployer,
            HasBlockRelease = hasBlockRelease,
            HasDayRelease = hasDayRelease
        };
        sut.HasMultipleDeliveryOptions.Should().Be(expected);
    }

    [TestCase("x", false)]
    [TestCase("x", false)]
    [TestCase("5", true)]
    [TestCase("5.05", true)]
    public void ThenHasAchievementRateIsAsExpected(string achievementRate, bool expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            AchievementRate = achievementRate
        };
        sut.HasAchievementRate.Should().Be(expected);
    }

    [TestCase("location desc", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void ThenHasLocationIsAsExpected(string locationDescription, bool expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            LocationDescription = locationDescription
        };
        sut.HasLocation.Should().Be(expected);
    }

    [TestCase(CourseType.Apprenticeship, true, false, false, false, false, 1)]
    [TestCase(CourseType.Apprenticeship, false, true, false, false, false, 1)]
    [TestCase(CourseType.Apprenticeship, false, false, true, false, false, 1)]
    [TestCase(CourseType.Apprenticeship, false, false, false, true, false, 0)]
    [TestCase(CourseType.Apprenticeship, false, false, false, false, true, 0)]
    [TestCase(CourseType.Apprenticeship, true, true, true, true, true, 3)]
    [TestCase(CourseType.ShortCourse, true, false, false, false, false, 1)]
    [TestCase(CourseType.ShortCourse, false, true, false, false, false, 0)]
    [TestCase(CourseType.ShortCourse, false, false, true, false, false, 0)]
    [TestCase(CourseType.ShortCourse, false, false, false, true, false, 1)]
    [TestCase(CourseType.ShortCourse, false, false, false, false, true, 1)]
    [TestCase(CourseType.ShortCourse, true, true, true, true, true, 3)]
    public void NoOfDeliveryOptions_CourseTypeAndDeliveryOptionsVary_ReturnsExpectedCount(CourseType courseType, bool atEmployer, bool hasDayRelease, bool hasBlockRelease, bool atProviderLocation, bool hasOnlineDeliveryOption, int expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            CourseType = courseType,
            AtEmployer = atEmployer,
            HasDayRelease = hasDayRelease,
            HasBlockRelease = hasBlockRelease,
            AtProvider = atProviderLocation,
            HasOnlineDeliveryOption = hasOnlineDeliveryOption
        };

        sut.NoOfDeliveryOptions().Should().Be(expected);
    }

    [TestCase(false, false, false, true, false)]
    [TestCase(false, false, false, false, true)]
    [TestCase(true, false, false, false, true)]
    public void HasMultipleDeliveryOptions_DeliveryOptionsVary_ReturnsExpectedValue(bool atEmployer, bool hasDayRelease, bool hasBlockRelease, bool atProviderLocation, bool hasOnlineDeliveryOption)
    {
        ShortlistProviderViewModel sut = new()
        {
            AtEmployer = atEmployer,
            HasDayRelease = hasDayRelease,
            HasBlockRelease = hasBlockRelease,
            AtProvider = atProviderLocation,
            HasOnlineDeliveryOption = hasOnlineDeliveryOption
        };

        sut.HasMultipleDeliveryOptions.Should().Be(sut.NoOfDeliveryOptions() > 1);
    }

    [TestCase(CourseType.ShortCourse, true)]
    [TestCase(CourseType.Apprenticeship, false)]
    public void IsShortCourseType_CourseTypeVaries_ReturnsExpectedValue(CourseType courseType, bool expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            CourseType = courseType
        };

        sut.IsShortCourseType.Should().Be(expected);
    }

}



