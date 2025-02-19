using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses;

public sealed class ValidDistancesTests
{
    [Test]
    public void IsValidDistance_ShouldReturnFalse_ForMissSpelledAcrossEngland()
    {
        Assert.That(ValidDistances.IsValidDistance("alll"), Is.False);
    }

    [Test]
    public void IsValidDistance_ShouldReturnTrue_ForValidDistances()
    {
        var validDistances = new List<string> { "2", "5", "10", "15", "20", "30", "40", "50", "100" };
        foreach (var distance in validDistances)
        {
            Assert.That(ValidDistances.IsValidDistance(distance), Is.True);
        }
    }

    [Test]
    public void IsValidDistance_ShouldReturnFalse_ForInvalidDistances()
    {
        var invalidDistances = new List<string> { "1", "3", "25", "60", "101", "-10", "0" };
        foreach (var distance in invalidDistances)
        {
            Assert.That(ValidDistances.IsValidDistance(distance), Is.False);
        }
    }

    [Test]
    public void Distances_ShouldContain_AllValidValues()
    {
        var expectedDistances = new HashSet<int> { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
        var actualDistances = ValidDistances.Distances;
        Assert.That(expectedDistances, Is.EquivalentTo(actualDistances));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnDefault_WhenLocationIsNotSet()
    {
        Assert.That(ValidDistances.GetValidDistance("40", ""), Is.EqualTo(ValidDistances.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnValidDistance_WhenLocationIsSet()
    {
        Assert.That(ValidDistances.GetValidDistance("40", "location"), Is.EqualTo(40));
    }

    [Test]
    public void GetValidDistance_LocationOverload_ShouldReturnDefault_WhenDistanceFilterIsAll()
    {
        Assert.That(ValidDistances.GetValidDistance("All", "location"), Is.EqualTo(ValidDistances.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetValidDistance_ShouldReturnParsedDistance_WhenDistanceIsValid()
    {
        Assert.That(ValidDistances.GetValidDistance("40"), Is.EqualTo(40));
    }

    [Test]
    public void GetValidDistance_ShouldReturnDefault_WhenDistanceIsNotValid()
    {
        Assert.That(ValidDistances.GetValidDistance("41"), Is.EqualTo(ValidDistances.DEFAULT_DISTANCE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenLocationIsEmpty()
    {
        Assert.That(ValidDistances.GetDistanceQueryString("40", string.Empty), Is.EqualTo(ValidDistances.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenLocationIsNull()
    {
        Assert.That(ValidDistances.GetDistanceQueryString("40", null), Is.EqualTo(ValidDistances.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldReturnAcrossEnglandFilterValue_WhenDistanceIsInvalid()
    {
        Assert.That(ValidDistances.GetDistanceQueryString("41", "location"), Is.EqualTo(ValidDistances.ACROSS_ENGLAND_FILTER_VALUE));
    }

    [Test]
    public void GetDistanceQueryString_ShouldStringDistance_WhenDistanceIsValid()
    {
        Assert.That(ValidDistances.GetDistanceQueryString("40", "location"), Is.EqualTo("40"));
    }
}
