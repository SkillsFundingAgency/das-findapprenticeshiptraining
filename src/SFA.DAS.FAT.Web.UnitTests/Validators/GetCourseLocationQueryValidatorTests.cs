using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Validators;

public class GetCourseLocationQueryValidatorTests
{
    [Test]
    [MoqInlineAutoData]
    public async Task TestValidator_Location_Invalid_ReturnsExpectedErrorMessage(
        string location,
        [Frozen] Mock<ILocationService> locationServiceMock,
        [Greedy] GetCourseLocationQueryValidator validator
        )
    {
        locationServiceMock.Setup(x => x.IsLocationValid(location)).ReturnsAsync(false);
        var result = await validator.TestValidateAsync(new GetCourseLocationQuery { Location = location });
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Location)
            .WithErrorMessage(GetCourseLocationQueryValidator.LocationErrorMessage);
        locationServiceMock.Verify(x => x.IsLocationValid(It.IsAny<string>()), Times.Once);
    }


    [Test]
    [MoqInlineAutoData(null)]
    [MoqInlineAutoData("")]
    [MoqInlineAutoData("      ")]
    public async Task TestValidator_Location_Empty_Returns_NoError(
        string location,
        [Frozen] Mock<ILocationService> locationServiceMock,
        [Greedy] GetCourseLocationQueryValidator validator
    )
    {
        var result = await validator.TestValidateAsync(new GetCourseLocationQuery { Location = location });
        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(c => c.Location);
        locationServiceMock.Verify(x => x.GetLocations(It.IsAny<string>()), Times.Never);
    }
}
