using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheReviewsModel
{
    [TestCase("1920", "2019 to 2020")]
    [TestCase("0001", "2000 to 2001")]
    [TestCase("2122", "2021 to 2022")]
    public void Then_Academic_Period_Should_Return_Formatted_Period_When_Valid(string input, string expected)
    {
        var sut = new ReviewsModel
        {
            ReviewPeriod = input
        };

        Assert.That(sut.AcademicPeriod, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    [TestCase("21")] 
    [TestCase("abcd")]
    [TestCase("21AB")]
    public void Then_Academic_Period_Should_Return_Property_Value_When_Invalid(string invalidInput)
    {
        var sut = new ReviewsModel
        {
            ReviewPeriod = invalidInput
        };

        Assert.That(sut.AcademicPeriod, Is.EqualTo(invalidInput));
    }

    [TestCase("5", 5)]
    [TestCase("0", 0)]
    [TestCase("10", 10)]
    public void Then_Converted_Employer_Stars_Should_Return_Parsed_Int_When_Valid(string input, int expected)
    {
        var sut = new ReviewsModel
        {
            EmployerStars = input
        };

        Assert.That(sut.ConvertedEmployerStars, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("invalidvalue")]
    public void Then_Converted_Employer_Stars_Should_Return_Zero_When_Invalid(string input)
    {
        var sut = new ReviewsModel
        {
            EmployerStars = input
        };

        Assert.That(sut.ConvertedEmployerStars, Is.EqualTo(0));
    }

    [TestCase("4", 4)]
    [TestCase("1", 1)]
    [TestCase("9", 9)]
    public void Converted_Apprentice_Stars_Should_Return_Parsed_Int_When_Valid(string input, int expected)
    {
        var sut = new ReviewsModel
        {
            ApprenticeStars = input
        };

        Assert.That(sut.ConvertedApprenticeStars, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("NaN")]
    public void Converted_Apprentice_Stars_Should_Return_Zero_When_Invalid(string input)
    {
        var sut = new ReviewsModel
        {
            ApprenticeStars = input
        };

        Assert.That(sut.ConvertedApprenticeStars, Is.EqualTo(0));
    }
}
