using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistsViewModelTests;

public class WhenCreatingShortlistProviderViewModel
{
    [TestCase(1, "Block release")]
    [TestCase(2, "Block release at multiple locations")]
    public void ThenBlockReleaseTextIsAsExpected(int count, string expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            HasBlockRelease = true,
            BlockReleaseDistance = 10,
            BlockReleaseCount = count
        };
        sut.BlockReleaseText.Should().Be(expected);
    }

    [TestCase(1, "Day release")]
    [TestCase(2, "Day release at multiple locations")]
    public void ThenDayReleaseTextIsAsExpected(int count, string expected)
    {
        ShortlistProviderViewModel sut = new()
        {
            HasDayRelease = true,
            DayReleaseDistance = 10,
            DayReleaseCount = count
        };
        sut.DayReleaseText.Should().Be(expected);
    }

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
        sut.NoOfDeliveryOptions.Should().Be(expected);
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
}



