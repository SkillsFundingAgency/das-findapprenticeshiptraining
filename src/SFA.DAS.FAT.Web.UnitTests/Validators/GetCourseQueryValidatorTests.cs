﻿using FluentValidation.TestHelper;
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
    public void TestValidator_CourseIdInvalid_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseQuery());

        Assert.IsFalse(result.IsValid);
        result.ShouldHaveValidationErrorFor(c => c.CourseId)
            .WithErrorMessage(GetCourseQueryValidator.CourseIdErrorMessage);
    }

    [Test]
    public void TestValidator_CourseIdValid_ReturnsValid()
    {
        var result = _validator.TestValidate(new GetCourseQuery { CourseId = 1 });

        Assert.IsTrue(result.IsValid);
    }
}
