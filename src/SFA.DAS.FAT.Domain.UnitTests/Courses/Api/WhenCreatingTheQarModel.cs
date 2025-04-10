using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheQarModel
{
    [TestCase("25", 25)]
    [TestCase("0", 0)]
    [TestCase(null, 0)]
    [TestCase("abc", 0)]
    public void Then_Converted_Leavers_Should_Return_Parsed_Or_Zero(string leavers, int expected)
    {
        var model = new QarModel { Leavers = leavers };

        Assert.That(model.ConvertedLeavers, Is.EqualTo(expected));
    }

    [TestCase("85.6", 85.6)]
    [TestCase("100", 100)]
    [TestCase(null, null)]
    [TestCase("invalid", null)]
    public void Then_Converted_Achievement_Rate_Should_Return_Parsed_Or_Null(string rate, decimal? expected)
    {
        var sut = new QarModel { AchievementRate = rate };

        Assert.That(sut.ConvertedAchievementRate, Is.EqualTo(expected));
    }

    [TestCase("25", "50", 50)]
    [TestCase("20", "100", 20)]
    [TestCase("20", "0", 20)]
    [TestCase("0", "80", 0)]
    [TestCase("abc", "90", 0)]
    [TestCase("25", "abc", 0)]
    public void Then_Total_Participants_Should_Calculate_Correctly(string leavers, string rate, int expected)
    {
        var sut = new QarModel
        {
            Leavers = leavers,
            AchievementRate = rate
        };

        Assert.That(sut.TotalParticipants, Is.EqualTo(expected));
    }

    [TestCase("90", 10)]
    [TestCase("100", 0)]
    [TestCase("0", 100)]
    public void Then_Failure_Rate_Should_Return_One_Hundred_Minus_Achievement_Rate(string rate, decimal expected)
    {
        var sut = new QarModel
        {
            AchievementRate = rate
        };

        Assert.That(sut.FailureRate, Is.EqualTo(expected));
    }

    [Test]
    public void Failure_Rate_Should_Return_Zero_When_Achievement_Rate_Is_Null()
    {
        var sut = new QarModel { AchievementRate = null };

        Assert.That(sut.FailureRate, Is.EqualTo(0));
    }

    [TestCase("1819", "2018", "2019", "2018 to 2019")]
    [TestCase("9900", "2099", "2000", "2099 to 2000")]
    public void Then_Period_Display_Should_Extract_Years_Correctly(string period, string startYear, string endYear, string display)
    {
        var sut = new QarModel { Period = period };

        Assert.Multiple(() =>
        {
            Assert.That(sut.QarPeriodStartYear, Is.EqualTo(startYear));
            Assert.That(sut.QarPeriodEndYear, Is.EqualTo(endYear));
            Assert.That(sut.PeriodDisplay, Is.EqualTo(display));
        });
    }
}
