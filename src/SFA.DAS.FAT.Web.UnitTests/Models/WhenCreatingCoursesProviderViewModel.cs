using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingCoursesProviderViewModel
{
    private const string NoAchievementRateMessage = "No achievement rate - not enough data";

    [Test, AutoData]
    public void Then_The_Fields_Are_Correctly_Mapped(ProviderData source)
    {
        var actual = (CoursesProviderViewModel)source;
        actual.Should().BeEquivalentTo(source);
    }

    [Test]
    public void Then_A_Null_Source_Returns_Null_ViewModel()
    {
        var actual = (CoursesProviderViewModel)(ProviderData)null;
        actual.Should().BeNull();
    }

    [Test]
    [MoqInlineAutoData(LocationType.National, true, true)]
    [MoqInlineAutoData(LocationType.Regional, true, true)]
    [MoqInlineAutoData(LocationType.Provider, true, true)]
    [MoqInlineAutoData(LocationType.Online, true, true)]
    [MoqInlineAutoData(LocationType.National, false, false)]
    [MoqInlineAutoData(LocationType.Regional, false, false)]
    [MoqInlineAutoData(LocationType.Provider, false, false)]
    [MoqInlineAutoData(LocationType.Online, false, false)]
    public void Then_Set_EmployerLocationAvailable_If_There_Is_One_Location_At_Employer(LocationType employerLocationType, bool isBlockReleaseAvailable, bool isDayReleaseAvailable)
    {
        var locations = new List<ProviderLocation>
        {
            new() {LocationType = employerLocationType},
            new() {BlockRelease = isBlockReleaseAvailable},
            new() {DayRelease = isDayReleaseAvailable}
        };

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        var isEmployerAvailable = employerLocationType == LocationType.National || employerLocationType == LocationType.Regional;
        sut.IsEmployerLocationAvailable.Should().Be(isEmployerAvailable);
        sut.IsBlockReleaseAvailable.Should().Be(isBlockReleaseAvailable);
        sut.IsDayReleaseAvailable.Should().Be(isDayReleaseAvailable);
    }

    [TestCase("10", "40", "10% (out of 40 apprentices)")]
    [TestCase("15", "40", "15% (out of 40 apprentices)")]
    [TestCase("10", "427", "10% (out of 427 apprentices)")]
    [TestCase("low", "40", NoAchievementRateMessage)]
    [TestCase("-", "40", NoAchievementRateMessage)]
    [TestCase("10", "x", NoAchievementRateMessage)]
    [TestCase("10", "-", NoAchievementRateMessage)]
    [TestCase("low", "x", NoAchievementRateMessage)]
    [TestCase("low", "-", NoAchievementRateMessage)]
    [TestCase("-", "x", NoAchievementRateMessage)]
    [TestCase("-", "-", NoAchievementRateMessage)]
    public void Then_Set_AchievementRateMessage_For_AchievementRate_And_Leavers(string achievementRate, string leavers, string expectedMessage)
    {
        var sut = new CoursesProviderViewModel
        {
            AchievementRate = achievementRate,
            Leavers = leavers
        };

        sut.AchievementRateMessage.Should().Be(expectedMessage);
    }

    [TestCase(null, null, LocationType.National, null)]
    [TestCase(1.5, null, LocationType.Provider, null)]
    [TestCase(1.5, null, LocationType.National, 1.5)]
    [TestCase(1.5, 2.5, LocationType.National, 1.5)]
    [TestCase(2.5, 1.5, LocationType.Regional, 1.5)]
    public void Then_NearestEmployerLocation_Is_set(decimal? courseDistance, decimal? secondCourseDistance, LocationType locationType, decimal? expectedValue)
    {
        var locations = new List<ProviderLocation>();

        if (courseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)courseDistance, LocationType = locationType });
        }

        if (secondCourseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)secondCourseDistance, LocationType = locationType });
        }

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        sut.NearestEmployerLocation.Should().Be(expectedValue);
    }

    [TestCase(null, null, true, null)]
    [TestCase(1.5, null, false, null)]
    [TestCase(1.5, null, true, 1.5)]
    [TestCase(1.5, 2.5, true, 1.5)]
    [TestCase(2.5, 1.5, true, 1.5)]
    public void Then_NearestBlockRelease_Is_set(decimal? courseDistance, decimal? secondCourseDistance, bool blockRelease, decimal? expectedValue)
    {
        var locations = new List<ProviderLocation>();

        if (courseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)courseDistance, BlockRelease = blockRelease });
        }

        if (secondCourseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)secondCourseDistance, BlockRelease = blockRelease });
        }

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        sut.NearestBlockRelease.Should().Be(expectedValue);
    }

    [TestCase(null, null, true, null)]
    [TestCase(1.5, null, false, null)]
    [TestCase(1.5, null, true, 1.5)]
    [TestCase(1.5, 2.5, true, 1.5)]
    [TestCase(2.5, 1.5, true, 1.5)]
    public void Then_NearestDayRelease_Is_set(decimal? courseDistance, decimal? secondCourseDistance, bool dayRelease, decimal? expectedValue)
    {
        var locations = new List<ProviderLocation>();

        if (courseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)courseDistance, DayRelease = dayRelease });
        }

        if (secondCourseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)secondCourseDistance, DayRelease = dayRelease });
        }

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        sut.NearestDayRelease.Should().Be(expectedValue);
    }


    [TestCase(null, null, true, false)]
    [TestCase(1.5, null, false, false)]
    [TestCase(1.5, 2.5, false, false)]
    [TestCase(1.5, 2.5, true, true)]
    [TestCase(2.5, 1.5, true, true)]
    public void Then_DayReleaseMultiple_Is_set(decimal? courseDistance, decimal? secondCourseDistance, bool dayRelease, bool expectedValue)
    {
        var locations = new List<ProviderLocation>();

        if (courseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)courseDistance, DayRelease = dayRelease });
        }

        if (secondCourseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)secondCourseDistance, DayRelease = dayRelease });
        }

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        sut.IsDayReleaseMultiple.Should().Be(expectedValue);
    }


    [TestCase(null, null, true, false)]
    [TestCase(1.5, null, false, false)]
    [TestCase(1.5, 2.5, false, false)]
    [TestCase(1.5, 2.5, true, true)]
    [TestCase(2.5, 1.5, true, true)]
    public void Then_BlockReleaseMultiple_Is_set(decimal? courseDistance, decimal? secondCourseDistance, bool blockRelease, bool expectedValue)
    {
        var locations = new List<ProviderLocation>();
        if (courseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)courseDistance, BlockRelease = blockRelease });
        }

        if (secondCourseDistance != null)
        {
            locations.Add(new ProviderLocation { CourseDistance = (decimal)secondCourseDistance, BlockRelease = blockRelease });
        }

        var providerData = new ProviderData { Locations = locations };

        var sut = (CoursesProviderViewModel)providerData;

        sut.IsBlockReleaseMultiple.Should().Be(expectedValue);
    }
}
