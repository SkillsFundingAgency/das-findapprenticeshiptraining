using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistsViewModelTests;

public class WhenCreatingShortlistsViewModel
{
    [TestCase("provider name", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void ThenShowRemovedShortlistBannerIsAsExpected(string removedProviderName, bool expected)
    {
        ShortlistsViewModel sut = new()
        {
            RemovedProviderName = removedProviderName
        };

        sut.ShowRemovedShortlistBanner.Should().Be(expected);
    }
    [Test]
    public void HasShortlistItems_CoursesCollectionIsEmpty_ReturnsFalse()
    {
        ShortlistsViewModel sut = new();

        sut.HasShortlistItems.Should().BeFalse();
    }

    [Test]
    public void HasShortlistItems_CoursesCollectionContainsItem_ReturnsTrue()
    {
        ShortlistsViewModel sut = new()
        {
            Courses = new List<ShortlistCourseViewModel> { new ShortlistCourseViewModel() }
        };

        sut.HasShortlistItems.Should().BeTrue();
    }

    [TestCase(CourseType.ShortCourse, true)]
    [TestCase(CourseType.Apprenticeship, false)]
    public void IsShortCourseType_CourseTypeVaries_ReturnsExpectedValue(CourseType courseType, bool expected)
    {
        ShortlistCourseViewModel sut = new()
        {
            CourseType = courseType
        };

        sut.IsShortCourseType.Should().Be(expected);
    }

    [TestCase(LearningType.Apprenticeship, "govuk-tag--blue")]
    [TestCase(LearningType.FoundationApprenticeship, "govuk-tag--pink")]
    [TestCase(LearningType.ApprenticeshipUnit, "govuk-tag--purple")]
    public void LearningTypeTagClass_LearningTypeVaries_ReturnsExpectedCssClass(LearningType learningType, string expected)
    {
        ShortlistCourseViewModel sut = new()
        {
            LearningType = learningType
        };

        sut.LearningTypeTagClass.Should().Be(expected);
    }

    [Test]
    public void Providers_DefaultConstruction_InitializesAsEmptyList()
    {
        ShortlistLocationViewModel sut = new();

        sut.Providers.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void RequestApprenticeshipTraining_SetValues_RetainsValues()
    {
        RequestApprenticeshipTrainingViewModel sut = new()
        {
            CourseTitle = "Course title",
            Url = "https://example.test/request"
        };

        sut.CourseTitle.Should().Be("Course title");
        sut.Url.Should().Be("https://example.test/request");
    }
}
