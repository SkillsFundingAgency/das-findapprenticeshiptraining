using NUnit.Framework;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses;

public sealed class ValidDistancesTests
{
    [Test]
    public void IsValidDistance_ShouldReturnFalse_ForMissSpelledAcrossEngland()
    {
        Assert.That(DistanceService.IsValidDistance("alll"), Is.False);
    }

    [Test]
    public void IsValidDistance_ShouldReturnTrue_ForValidDistances()
    {
        var validDistances = new List<string> { "2", "5", "10", "15", "20", "30", "40", "50", "100" };
        foreach (var distance in validDistances)
        {
            Assert.That(DistanceService.IsValidDistance(distance), Is.True);
        }
    }

    [Test]
    public void IsValidDistance_ShouldReturnFalse_ForInvalidDistances()
    {
        var invalidDistances = new List<string> { "1", "3", "25", "60", "101", "-10", "0" };
        foreach (var distance in invalidDistances)
        {
            Assert.That(DistanceService.IsValidDistance(distance), Is.False);
        }
    }

    [Test]
    public void Distances_ShouldContain_AllValidValues()
    {
        var expectedDistances = new HashSet<int> { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
        var actualDistances = DistanceService.Distances;
        Assert.That(expectedDistances, Is.EquivalentTo(actualDistances));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnDefault_WhenLocationIsNotSet()
    {
        Assert.That(DistanceService.GetValidDistance("40", ""), Is.EqualTo(DistanceService.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnValidDistance_WhenLocationIsSet()
    {
        Assert.That(DistanceService.GetValidDistance("40", "location"), Is.EqualTo(40));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnDefault_WhenDistanceFilterIsAll()
    {
        Assert.That(DistanceService.GetValidDistance("All", "location"), Is.EqualTo(DistanceService.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetValidDistance_ShouldReturnParsedDistance_WhenDistanceIsValid()
    {
        Assert.That(DistanceService.GetValidDistance("40"), Is.EqualTo(40));
    }

    [Test]
    public void GetValidDistance_ShouldReturnDefault_WhenDistanceIsNotValid()
    {
        Assert.That(DistanceService.GetValidDistance("41"), Is.EqualTo(DistanceService.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetValidDistanceNullable_ShouldReturnParsedDistance_WhenDistanceIsValid()
    {
        Assert.That(DistanceService.GetValidDistanceNullable("40"), Is.EqualTo(40));
    }

    [Test]
    public void GetValidDistanceNullable_ShouldReturnDefault_WhenDistanceIsNotValid()
    {
        Assert.That(DistanceService.GetValidDistanceNullable("41"), Is.Null);
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenLocationIsEmpty()
    {
        Assert.That(DistanceService.GetDistanceQueryString("40", string.Empty), Is.EqualTo(DistanceService.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenLocationIsNull()
    {
        Assert.That(DistanceService.GetDistanceQueryString("40", null), Is.EqualTo(DistanceService.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenDistanceIsInvalid()
    {
        Assert.That(DistanceService.GetDistanceQueryString("41", "location"), Is.EqualTo(DistanceService.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldStringDistance_WhenDistanceIsValid()
    {
        Assert.That(DistanceService.GetDistanceQueryString("40", "location"), Is.EqualTo("40"));
    }
}
