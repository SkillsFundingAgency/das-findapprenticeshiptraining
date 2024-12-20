using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Web.Validators;

namespace SFA.DAS.FAT.Web.UnitTests.Validators
{
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
            var result = _validator.TestValidate(new GetCourseProviderQuery());

            Assert.IsFalse(result.IsValid);
            result.ShouldHaveValidationErrorFor(c => c.ProviderId)
                .WithErrorMessage(GetCourseProviderDetailsQueryValidator.ProviderIdErrorMessage);
        }

        [Test]
        public void TestValidator_ProviderIdValid_ReturnsValid()
        {
            var result = _validator.TestValidate(new GetCourseProviderQuery { ProviderId = 1 });

            Assert.IsTrue(result.IsValid);
        }
    }
}
