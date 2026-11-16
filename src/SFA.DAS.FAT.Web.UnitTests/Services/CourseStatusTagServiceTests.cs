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
        var result = CourseStatusTagService.GetTag(
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
        var result = CourseStatusTagService.GetTag(
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
        var result = CourseStatusTagService.GetTag(
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
        var result = CourseStatusTagService.GetTag(
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
        var result = CourseStatusTagService.GetTagText(CourseStatusTag.NewCourse);

        result.Should().Be(CourseStatusTagService.NewCourseText);
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosedToNewLearners_ThenReturnsGreyTagClass()
    {
        var result = CourseStatusTagService.GetTagClass(CourseStatusTag.ClosedToNewLearners);

        result.Should().Be("govuk-tag--grey");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosingToNewLearnersSoon_ThenReturnsOrangeTagClass()
    {
        var result = CourseStatusTagService.GetTagClass(CourseStatusTag.ClosingToNewLearnersSoon);

        result.Should().Be("govuk-tag--orange");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsNewCourse_ThenReturnsYellowTagClass()
    {
        var result = CourseStatusTagService.GetTagClass(CourseStatusTag.NewCourse);

        result.Should().Be("govuk-tag--yellow");
    }
}
