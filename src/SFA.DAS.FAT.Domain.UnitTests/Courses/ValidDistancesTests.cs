using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses;

public sealed class ValidDistancesTests
{
    [Test]
    public void IsValidDistance_ShouldReturnTrue_ForValidDistances()
    {
        var validDistances = new List<int> { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
        foreach (var distance in validDistances)
        {
            Assert.That(ValidDistances.IsValidDistance(distance), Is.True);
        }
    }

    [Test]
    public void IsValidDistance_ShouldReturnFalse_ForInvalidDistances()
    {
        var invalidDistances = new List<int> { 1, 3, 25, 60, 101, -10, 0 };
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
}
