using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Web.Validators;

namespace SFA.DAS.FAT.Web.UnitTests.Validators;

public class GetCourseProviderDetailsQueryValidatorTests
{
    private GetCourseProviderDetailsQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new GetCourseProviderDetailsQueryValidator();
    }

    [Test]
    public void TestValidator_ProviderIdInvalid_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new GetCourseProviderDetailsQuery());

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Ukprn)
            .WithErrorMessage(GetCourseProviderDetailsQueryValidator.ProviderIdErrorMessage);
    }

    [Test]
    public void TestValidator_ProviderIdValid_ReturnsValid()
    {
        var result = _validator.TestValidate(new GetCourseProviderDetailsQuery { Ukprn = 1 });

        result.IsValid.Should().BeTrue();
    }
}
