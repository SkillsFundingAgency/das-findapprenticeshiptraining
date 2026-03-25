using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingStandardViewModel
{
    [TestCase(ApprenticeshipType.Apprenticeship, "govuk-tag--blue")]
    [TestCase(ApprenticeshipType.FoundationApprenticeship, "govuk-tag--pink")]
    [TestCase(ApprenticeshipType.ApprenticeshipUnit, "govuk-tag--purple")]
    public void ApprenticeshipTypeTagClass_ForKnownType_ReturnsExpectedCssClass(
        ApprenticeshipType apprenticeshipType,
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
            ApprenticeshipType = apprenticeshipType
        };

        sut.ApprenticeshipTypeTagClass.Should().Be(expectedCssClass);
    }

    [Test, AutoData]
    public void ApprenticeshipTypeTagClass_ForUnknownType_ReturnsEmptyString(StandardViewModel model)
    {
        model.ApprenticeshipType = (ApprenticeshipType)999;


        model.ApprenticeshipTypeTagClass.Should().Be(string.Empty);
    }

    [Test, AutoData]
    public void ImplicitOperator_FromStandardModel_MapsAllFields(StandardModel source)
    {
        StandardViewModel sut = source;

        sut.Should().NotBeNull();
        sut.Should().BeEquivalentTo(source);
    }
}
