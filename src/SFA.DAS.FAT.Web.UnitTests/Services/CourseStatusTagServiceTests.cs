using FluentAssertions;
using FluentAssertions.Execution;
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

    [Test]
    public void WhenGettingProviderCourseAddedDescription_AndDateProvided_ThenReturnsFormattedDescription()
    {
        var result = CourseStatusTagService.GetProviderCourseAddedDescription(
            new DateTime(2026, 6, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course was added by the training provider on 13 Jun 2026.");
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsInTheFuture_ThenReturnsFormattedDescription()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);
        var result = CourseStatusTagService.GetProviderCourseClosingDescription(
            new DateTime(2027, 3, 27, 0, 0, 0, DateTimeKind.Utc),
            currentDate);

        result.Should().Be("This training provider will stop offering this course on 27 Mar 2027");
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsInThePast_ThenReturnsEmptyString()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);
        var result = CourseStatusTagService.GetProviderCourseClosingDescription(
            new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc),
            currentDate);

        result.Should().BeEmpty();
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsNull_ThenReturnsEmptyString()
    {
        var result = CourseStatusTagService.GetProviderCourseClosingDescription(
            null,
            new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeEmpty();
    }

    [Test]
    public void WhenHasProviderCourseClosingDate_AndDateIsToday_ThenReturnsTrue()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseStatusTagService.HasProviderCourseClosingDate(currentDate, currentDate);

        result.Should().BeTrue();
    }

    [Test]
    public void WhenHasProviderCourseClosingDate_AndDateIsInThePast_ThenReturnsFalse()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseStatusTagService.HasProviderCourseClosingDate(
            currentDate.AddDays(-1),
            currentDate);

        result.Should().BeFalse();
    }

    [Test]
    public void WhenFormattingDate_ThenReturnsDayMonthYearFormat()
    {
        using (new AssertionScope())
        {
            CourseStatusTagService.FormatDate(new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc))
                .Should().Be("16 Jul 2027");
            CourseStatusTagService.FormatDate(new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc))
                .Should().Be("27 Mar 2026");
        }
    }
}
