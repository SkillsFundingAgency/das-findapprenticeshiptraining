using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class CourseStatusTagServiceTests
{
    [Test]
    public void WhenGettingTag_AndLastDateStartsMatchesEffectiveFrom_ThenReturnsNewCourseTag()
    {
        var result = CourseStatusTagService.GetCourseStatusTag(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseStatusTag.NewCourse);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsInFutureAndDiffersFromEffectiveFrom_ThenReturnsClosingSoonTag()
    {
        var result = CourseStatusTagService.GetCourseStatusTag(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseStatusTag.ClosingToNewLearnersSoon);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsInPastAndDiffersFromEffectiveFrom_ThenReturnsClosedTag()
    {
        var result = CourseStatusTagService.GetCourseStatusTag(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseStatusTag.ClosedToNewLearners);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsNull_ThenReturnsNoTag()
    {
        var result = CourseStatusTagService.GetCourseStatusTag(
            new StandardDates
            {
                LastDateStarts = null,
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingTagText_AndTagIsNewCourse_ThenReturnsNewCourseText()
    {
        var result = CourseStatusTagService.GetCourseStatusTagText(CourseStatusTag.NewCourse);

        result.Should().Be(CourseStatusTagService.NewCourseText);
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosedToNewLearners_ThenReturnsGreyTagClass()
    {
        var result = CourseStatusTagService.GetCourseStatusTagClass(CourseStatusTag.ClosedToNewLearners);

        result.Should().Be("govuk-tag--grey");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosingToNewLearnersSoon_ThenReturnsOrangeTagClass()
    {
        var result = CourseStatusTagService.GetCourseStatusTagClass(CourseStatusTag.ClosingToNewLearnersSoon);

        result.Should().Be("govuk-tag--orange");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsNewCourse_ThenReturnsYellowTagClass()
    {
        var result = CourseStatusTagService.GetCourseStatusTagClass(CourseStatusTag.NewCourse);

        result.Should().Be("govuk-tag--yellow");
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsNewCourse_ThenReturnsNewCourseDescription()
    {
        var result = CourseStatusTagService.GetCourseStatusTagDescription(CourseStatusTag.NewCourse, null);

        result.Should().Be(CourseStatusTagService.NewCourseDescriptionText);
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsClosingToNewLearnersSoon_ThenReturnsClosingDescriptionWithFormattedDate()
    {
        var result = CourseStatusTagService.GetCourseStatusTagDescription(
            CourseStatusTag.ClosingToNewLearnersSoon,
            new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course is closing on 16 Jul 2027. Government funding will not be available for learners who start after this date.");
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsClosedToNewLearners_ThenReturnsClosedDescriptionWithFormattedDate()
    {
        var result = CourseStatusTagService.GetCourseStatusTagDescription(
            CourseStatusTag.ClosedToNewLearners,
            new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course stopped accepting new learners on 30 Jun 2026. Apprentices who are already on this course can still change training providers.");
    }
}
