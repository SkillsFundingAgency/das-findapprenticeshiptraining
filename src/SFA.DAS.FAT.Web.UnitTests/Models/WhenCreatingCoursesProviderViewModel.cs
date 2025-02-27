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
    [MoqInlineAutoData(true, true, true)]
    [MoqInlineAutoData(false, true, true)]
    [MoqInlineAutoData(true, false, true)]
    [MoqInlineAutoData(true, true, false)]
    [MoqInlineAutoData(true, false, false)]
    [MoqInlineAutoData(false, true, false)]
    [MoqInlineAutoData(false, false, true)]
    [MoqInlineAutoData(false, false, false)]
    public void Then_Set_EmployerLocationAvailable_If_There_Is_One_Location_At_Employer(bool isEmployerAvailable, bool isBlockReleaseAvailable, bool isDayReleaseAvailable)
    {
        var sut = new CoursesProviderViewModel
        {
            Locations = new List<ProviderLocation>
        {
            new() {AtEmployer = isEmployerAvailable},
            new() {BlockRelease = isBlockReleaseAvailable},
            new() {DayRelease = isDayReleaseAvailable}
        }
        };

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
}
