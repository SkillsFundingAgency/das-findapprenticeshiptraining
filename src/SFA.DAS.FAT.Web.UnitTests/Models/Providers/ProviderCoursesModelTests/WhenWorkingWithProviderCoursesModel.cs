using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Providers.ProviderCoursesModelTests;

public class WhenWorkingWithProviderCoursesModel
{
    [Test]
    public void CourseCount_WhenProviderCoursesModelCreated_ReturnsZero()
    {
        var sut = new ProviderCoursesModel();


        sut.CourseCount.Should().Be(0);
    }

    [Test]
    public void CoursesDropdownText_WhenProviderCoursesModelCreated_ReturnsEmptyString()
    {
        var sut = new ProviderCoursesModel();

        sut.CoursesDropdownText.Should().Be(string.Empty);
    }

    [Test]
    public void CoursesDropdownText_EmptyCourses_ReturnsZeroPluralMessage()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = new List<ProviderCourseDetails>()
        };

        sut.CourseCount.Should().Be(0);
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

    [TestCase(0, LearningType.ApprenticeshipUnit, 2, "Apprenticeship units", "Apprenticeship units")]
    [TestCase(1, LearningType.FoundationApprenticeship, 1, "Foundation apprenticeships", "Foundation apprenticeship")]
    [TestCase(2, LearningType.Apprenticeship, 1, "Apprenticeships", "Apprenticeship")]
    public void CourseGroups_CoursesWithDifferentTypes_GroupsInConfiguredOrderWithCorrectCountsAndDisplayNames(
        int groupIndex,
        LearningType expectedType,
        int expectedCount,
        string expectedDisplayNameHeader,
        string expectedDisplayName)
    {
        var courses = new List<ProviderCourseDetails>
        {
            new ProviderCourseDetails { ApprenticeshipType = LearningType.Apprenticeship },
            new ProviderCourseDetails { ApprenticeshipType = LearningType.ApprenticeshipUnit },
            new ProviderCourseDetails { ApprenticeshipType = LearningType.FoundationApprenticeship },
            new ProviderCourseDetails { ApprenticeshipType = LearningType.ApprenticeshipUnit }
        };

        var sut = new ProviderCoursesModel
        {
            Courses = courses
        };

        var groups = sut.CourseGroups;

        groups.Count.Should().Be(3);

        groups[groupIndex].ApprenticeshipType.Should().Be(ProviderCoursesModel.LearningTypeOrder[groupIndex]);
        groups[groupIndex].ApprenticeshipType.Should().Be(expectedType);
        groups[groupIndex].Count.Should().Be(expectedCount);
        groups[groupIndex].DisplayNameHeader.Should().Be(expectedDisplayNameHeader);
        groups[groupIndex].DisplayName.Should().Be(expectedDisplayName);
    }

    [Test]
    public void CourseGroups_DefaultCourses_ReturnsEmptyList()
    {
        var sut = new ProviderCoursesModel();

        var groups = sut.CourseGroups;

        groups.Should().NotBeNull();
        groups.Should().BeEmpty();
    }

    [Test]
    public void CourseGroups_EmptyCourses_ReturnsEmptyList()
    {
        var sut = new ProviderCoursesModel
        {
            Courses = new List<ProviderCourseDetails>()
        };

        var groups = sut.CourseGroups;

        groups.Should().NotBeNull();
        groups.Should().BeEmpty();
    }

    [Test]
    public void CourseGroups_SetsUkprnAndLocationOnGroups()
    {
        var courses = new List<ProviderCourseDetails>
        {
            new ProviderCourseDetails { ApprenticeshipType = LearningType.ApprenticeshipUnit },
            new ProviderCourseDetails { ApprenticeshipType = LearningType.FoundationApprenticeship }
        };

        var sut = new ProviderCoursesModel
        {
            Courses = courses,
            Ukprn = 123456,
            Location = "TestLocation"
        };

        var groups = sut.CourseGroups;

        groups.Should().NotBeNull();
        groups.Should().HaveCount(2);
        groups.All(g => g.Ukprn == 123456).Should().BeTrue();
        groups.All(g => g.Location == "TestLocation").Should().BeTrue();
    }

    [Test]
    public void ImplicitOperator_SourceNull_ReturnsModelWithEmptyCourses()
    {
        List<GetProviderCourseDetails> source = null;

        ProviderCoursesModel sut = source;

        sut.Should().NotBeNull();
        sut.Courses.Should().NotBeNull();
        sut.Courses.Should().BeEmpty();
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

    [Test]
    public void ImplicitOperator_FromProviderCourseDetails_SourceNull_ReturnsModelWithEmptyCourses()
    {
        List<ProviderCourseDetails> source = null;

        ProviderCoursesModel sut = source;

        sut.Should().NotBeNull();
        sut.Courses.Should().NotBeNull();
        sut.Courses.Should().BeEmpty();
        sut.CourseCount.Should().Be(0);
    }

    [Test]
    public void ImplicitOperator_FromProviderCourseDetails_SourceWithItems_MapsToModelCourses()
    {
        var source = new List<ProviderCourseDetails>
        {
            new ProviderCourseDetails { CourseName = "X", LarsCode = "10", Level = 3 },
            new ProviderCourseDetails { CourseName = "Y", LarsCode = "20", Level = 4 }
        };

        ProviderCoursesModel sut = source;

        sut.Should().NotBeNull();
        sut.Courses.Should().NotBeNull();
        sut.Courses.Count.Should().Be(source.Count);
        sut.Courses.Select(c => c.CourseName).Should().BeEquivalentTo(source.Select(s => s.CourseName));
    }
}
