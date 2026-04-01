using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingStandardViewModel
{
    [TestCase(LearningType.Apprenticeship, "govuk-tag--blue")]
    [TestCase(LearningType.FoundationApprenticeship, "govuk-tag--pink")]
    [TestCase(LearningType.ApprenticeshipUnit, "govuk-tag--purple")]
    public void LearningTypeTagClass_ForKnownType_ReturnsExpectedCssClass(
        LearningType learningType,
        string expectedCssClass)
    {
        var sut = new StandardViewModel
        {
            StandardUId = "ST0001",
            IfateReferenceNumber = "IFATE-0001",
            Title = "Title",
            OverviewOfRole = "Overview",
            Keywords = "Keywords",
            Route = "Route",
            LearningType = learningType
        };

        sut.LearningTypeTagClass.Should().Be(expectedCssClass);
    }

    [Test, AutoData]
    public void LearningTypeTagClass_ForUnknownType_ReturnsEmptyString(StandardViewModel model)
    {
        model.LearningType = (LearningType)999;


        model.LearningTypeTagClass.Should().Be(string.Empty);
    }

    [Test, AutoData]
    public void ImplicitOperator_FromStandardModel_MapsAllFields(StandardModel source)
    {
        StandardViewModel sut = source;

        sut.Should().NotBeNull();
        sut.Should().BeEquivalentTo(source);
    }
}
