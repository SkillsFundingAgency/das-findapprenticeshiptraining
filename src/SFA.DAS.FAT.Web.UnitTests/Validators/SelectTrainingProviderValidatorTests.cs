using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Validators;

namespace SFA.DAS.FAT.Web.UnitTests.Validators;
public class SelectTrainingProviderValidatorTests
{
    private SelectTrainingProviderValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new SelectTrainingProviderValidator();
    }

    [Test]
    public void TestValidator_ProviderIdInvalid_ReturnsExpectedErrorMessage()
    {
        var result = _validator.TestValidate(new SelectTrainingProviderSubmitViewModel());

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.SearchTerm)
            .WithErrorMessage(SelectTrainingProviderValidator.NoTrainingProviderSelectedErrorMessage);
    }

    [Test]
    public void TestValidator_ProviderIdValid_ReturnsValid()
    {
        var ukprn = "10001000";
        var result = _validator.TestValidate(new SelectTrainingProviderSubmitViewModel { Ukprn = ukprn });
        result.IsValid.Should().BeTrue();
    }
}
