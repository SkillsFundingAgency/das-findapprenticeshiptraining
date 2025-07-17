using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;
public class WhenBuildingStandardViewModelTests
{
    [Test]
    [MoqInlineAutoData(ApprenticeshipType.FoundationApprenticeship, "Foundation apprenticeship")]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, "Apprenticeship")]
    public void Then_Set_ApprenticeshipTypeDescription(ApprenticeshipType apprenticeshipType, string expectedDescription, StandardModel model)

    {
        model.ApprenticeshipType = apprenticeshipType;
        StandardViewModel sut = model;
        sut.ApprenticeTypeDescription.Should().Be(expectedDescription);
    }
}
