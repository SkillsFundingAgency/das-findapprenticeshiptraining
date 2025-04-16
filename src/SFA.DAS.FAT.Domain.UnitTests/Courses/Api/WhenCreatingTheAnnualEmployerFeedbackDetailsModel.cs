using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheAnnualEmployerFeedbackDetailsModel
{
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("  ", "")]
    [TestCase("All", "Overall reviews")]
    [TestCase("overall", "Overall reviews")]
    [TestCase("OVERALL", "Overall reviews")]
    [TestCase("FY1819", "2018 to 2019")]
    public void Then_Get_Converted_Time_Period_Returns_Expected_Output(string input, string expected)
    {
        var sut = new AnnualEmployerFeedbackDetailsModel()
        {
            TimePeriod = input
        };

        Assert.That(sut.TimePeriodDisplayText, Is.EqualTo(expected));
    }

    [Test]
    public void Then_Get_Converted_Time_Period_Returns_Today_When_End_Year_Is_Current_Year()
    {
        var currentYear = DateTime.UtcNow.Year;
        var shortCurrentYear = currentYear % 100;
        var timePeriod = $"FY{shortCurrentYear - 1}{shortCurrentYear:D2}";

        var sut = new AnnualEmployerFeedbackDetailsModel()
        {
            TimePeriod = timePeriod
        };

        Assert.That(sut.TimePeriodDisplayText, Is.EqualTo($"{currentYear - 1} to today"));
    }

    [Test]
    public void Then_Get_Converted_Time_Period_Returns_Original_When_Format_Is_Invalid()
    {
        var sut = new AnnualEmployerFeedbackDetailsModel()
        { 
            TimePeriod = "InvalidFormat" 
        };

        Assert.That(sut.TimePeriodDisplayText, Is.EqualTo("InvalidFormat"));
    }
}
