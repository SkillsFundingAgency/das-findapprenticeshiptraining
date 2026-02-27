using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Providers.ProviderCoursesModelTests;

public class WhenWorkingWithProviderCoursesModel
{
    [Test]
    public void CourseCount_CoursesNull_ReturnsZero()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = null
        };

        sut.CourseCount.Should().Be(0);
    }

    [Test]
    public void CoursesDropdownText_CoursesNull_ReturnsEmptyString()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = null
        };

        sut.CoursesDropdownText.Should().Be(string.Empty);
    }

    [Test]
    public void CoursesDropdownText_SingleCourse_ReturnsSingularMessage()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = new List<ProviderCourseDetails> { new ProviderCourseDetails() }
        };

        sut.CourseCount.Should().Be(1);
        sut.CoursesDropdownText.Should().Be("View 1 course delivered by this training provider");
    }

    [Test]
    public void CoursesDropdownText_MultipleCourses_ReturnsPluralMessage()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = new List<ProviderCourseDetails>
            {
                new ProviderCourseDetails(),
                new ProviderCourseDetails(),
                new ProviderCourseDetails()
            }
        };

        sut.CourseCount.Should().Be(3);
        sut.CoursesDropdownText.Should().Be("View 3 courses delivered by this training provider");
    }

    [TestCase(ApprenticeshipType.ApprenticeshipUnit, "Apprenticeship units")]
    [TestCase(ApprenticeshipType.FoundationApprenticeship, "Foundation apprenticeships")]
    [TestCase(ApprenticeshipType.Apprenticeship, "Apprenticeships")]
    public void GetDisplayName_ApprenticeshipType_ReturnsExpectedDisplayName(ApprenticeshipType type, string expected)
    {
        ProviderCoursesModel.GetDisplayName(type).Should().Be(expected);
    }

    [Test]
    public void GetCourseGroups_CoursesWithDifferentTypes_GroupsInConfiguredOrderWithCorrectCountsAndDisplayNames()
    {
        var courses = new List<ProviderCourseDetails>
        {
            new ProviderCourseDetails { ApprenticeshipType = ApprenticeshipType.Apprenticeship },
            new ProviderCourseDetails { ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit },
            new ProviderCourseDetails { ApprenticeshipType = ApprenticeshipType.FoundationApprenticeship },
            new ProviderCourseDetails { ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit }
        };

        var sut = new ProviderCoursesModel
        {
            Courses = courses
        };

        var groups = sut.GetCourseGroups();

        groups.Count.Should().Be(3);
        groups[0].ApprenticeshipType.Should().Be(ProviderCoursesModel.ApprenticeshipTypeOrder[0]);
        groups[1].ApprenticeshipType.Should().Be(ProviderCoursesModel.ApprenticeshipTypeOrder[1]);
        groups[2].ApprenticeshipType.Should().Be(ProviderCoursesModel.ApprenticeshipTypeOrder[2]);

        groups[0].Count.Should().Be(2);
        groups[1].Count.Should().Be(1);
        groups[2].Count.Should().Be(1);

        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].DisplayName.Should().Be(ProviderCoursesModel.GetDisplayName(groups[i].ApprenticeshipType));
        }
    }

    [Test]
    public void ImplicitOperator_SourceNull_ReturnsModelWithNullCourses()
    {
        List<GetProviderCourseDetails> source = null;

        ProviderCoursesModel sut = source;

        sut.Should().NotBeNull();
        sut.Courses.Should().BeNull();
        sut.CourseCount.Should().Be(0);
    }

    [Test]
    public void ImplicitOperator_SourceWithItems_MapsToModelCourses()
    {
        var source = new List<GetProviderCourseDetails>
        {
            new GetProviderCourseDetails { CourseName = "A", LarsCode = "1", Level = 1 },
            new GetProviderCourseDetails { CourseName = "B", LarsCode = "2", Level = 2 }
        };

        ProviderCoursesModel sut = source;

        sut.Should().NotBeNull();
        sut.Courses.Should().NotBeNull();
        sut.Courses.Count.Should().Be(source.Count);
    }
}
