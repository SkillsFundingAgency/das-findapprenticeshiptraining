using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;
public class WhenCalculatingStrengthWeaknessPercentagesForProviderAttributeTests
{
    [TestCase(1, 2, 33, 67)]
    [TestCase(0, 0, 0, 0)]
    [TestCase(20, 20, 50, 50)]
    [TestCase(20, 0, 100, 0)]
    [TestCase(0, 20, 0, 100)]
    [TestCase(21, 20, 51, 49)]
    [TestCase(20, 21, 49, 51)]
    public void Then_Percentage_Values_Should_Be_Expected(int strength, int weakness, int expectedStrengthPercentage, int expectedWeaknessPercentage)
    {
        var expectedTotalCount = strength + weakness;
        var attribute = new ProviderAttribute { Name = "x", Strength = strength, Weakness = weakness };
        attribute.TotalCount.Should().Be(expectedTotalCount);
        attribute.StrengthPerc.Should().Be(expectedStrengthPercentage);
        attribute.WeaknessPerc.Should().Be(expectedWeaknessPercentage);
    }
}
