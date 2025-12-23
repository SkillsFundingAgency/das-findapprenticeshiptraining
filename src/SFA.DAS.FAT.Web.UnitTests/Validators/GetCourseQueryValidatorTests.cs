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

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void LarsCode_InvalidValues_ShouldHaveValidationError(string larsCode)
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = larsCode });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.LarsCode)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [TestCase("1")]
    [TestCase("123")]
    [TestCase("2147483647")] // int.MaxValue
    public void LarsCode_ValidNumericStrings_ShouldBeValid(string larsCode)
    {
        var result = _validator.TestValidate(new GetCourseQuery { LarsCode = larsCode });

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
    }
}
