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

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(11)]
    [TestCase(111)]
    [TestCase(1111)]
    [TestCase(11111)]
    [TestCase(111111)]
    [TestCase(9999999)]
    [TestCase(20000000)]
    public void TestValidator_ProviderIdInvalid_ReturnsExpectedErrorMessage(int ukprn)
    {
        var result = _validator.TestValidate(new GetCourseProviderDetailsQuery { Ukprn = ukprn });

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Ukprn)
            .WithErrorMessage(GetCourseProviderDetailsQueryValidator.InvalidUkprnErrorMessage);
    }

    [TestCase(10000000)]
    [TestCase(19999999)]
    public void TestValidator_ProviderIdValid_ReturnsValid(int ukprn)
    {
        var result = _validator.TestValidate(new GetCourseProviderDetailsQuery { Ukprn = ukprn });

        result.IsValid.Should().BeTrue();
    }
}
