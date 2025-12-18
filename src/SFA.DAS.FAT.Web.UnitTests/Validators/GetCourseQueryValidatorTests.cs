using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Web.Validators;

namespace SFA.DAS.FAT.Web.UnitTests.Validators;

public class GetCourseQueryValidatorTests
{
    private GetCourseQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new GetCourseQueryValidator();
    }

    [Test]
    public void TestValidator_LarsCodeIsZero_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "0" });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsNegative_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "-1" });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsNull_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = null });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsEmpty_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = string.Empty });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsWhitespace_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "   " });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsNotNumeric_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "abc" });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsAlphanumeric_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "123abc" });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_LarsCodeIsValidPositiveInteger_ReturnsValid()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "1" });

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }

    [Test]
    public void TestValidator_LarsCodeIsValidLargeNumber_ReturnsValid()
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = "12345" });

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }

    [TestCase("123")]
    [TestCase("999")]
    [TestCase("1")]
    [TestCase("2147483647")] // int.MaxValue
    public void TestValidator_LarsCodeIsValidNumericString_ReturnsValid(string larsCode)
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = larsCode });

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }

    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("-100")]
    [TestCase("")]
    [TestCase("  ")]
    [TestCase("abc")]
    [TestCase("12.5")]
    [TestCase("2147483648")] // int.MaxValue + 1
    public void TestValidator_LarsCodeIsInvalid_ReturnsExpectedErrorMessage(string larsCode)
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = larsCode });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }
}
