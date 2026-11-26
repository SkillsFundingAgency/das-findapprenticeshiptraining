using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Helpers;

namespace SFA.DAS.FAT.Web.UnitTests.Helpers;

public class CourseFundingStatusHelperTests
{
    [Test]
    public void WhenGettingTag_AndLastDateStartsMatchesEffectiveFrom_ThenReturnsNewCourseTag()
    {
        var result = CourseFundingStatusHelper.GetCourseFundingStatus(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseFundingStatus.NewCourse);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsInFutureAndDiffersFromEffectiveFrom_ThenReturnsClosingSoonTag()
    {
        var result = CourseFundingStatusHelper.GetCourseFundingStatus(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseFundingStatus.ClosingToNewLearnersSoon);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsInPastAndDiffersFromEffectiveFrom_ThenReturnsClosedTag()
    {
        var result = CourseFundingStatusHelper.GetCourseFundingStatus(
            new StandardDates
            {
                LastDateStarts = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be(CourseFundingStatus.ClosedToNewLearners);
    }

    [Test]
    public void WhenGettingTag_AndLastDateStartsIsNull_ThenReturnsNoTag()
    {
        var result = CourseFundingStatusHelper.GetCourseFundingStatus(
            new StandardDates
            {
                LastDateStarts = null,
                EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingTag_AndCourseDatesIsNull_ThenReturnsNoTag()
    {
        var result = CourseFundingStatusHelper.GetCourseFundingStatus(
            null,
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingTagText_AndTagIsNewCourse_ThenReturnsNewCourseText()
    {
        CourseFundingStatus? courseFundingStatus = CourseFundingStatus.NewCourse;

        var result = courseFundingStatus.GetCourseFundingStatusDescription();

        result.Should().Be("New course");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosedToNewLearners_ThenReturnsGreyTagClass()
    {
        CourseFundingStatus? courseFundingStatus = CourseFundingStatus.ClosedToNewLearners;

        var result = courseFundingStatus.GetCourseFundingStatusTagClass();

        result.Should().Be("govuk-tag--grey");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosingToNewLearnersSoon_ThenReturnsOrangeTagClass()
    {
        CourseFundingStatus? courseFundingStatus = CourseFundingStatus.ClosingToNewLearnersSoon;

        var result = courseFundingStatus.GetCourseFundingStatusTagClass();

        result.Should().Be("govuk-tag--orange");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsNewCourse_ThenReturnsYellowTagClass()
    {
        CourseFundingStatus? courseFundingStatus = CourseFundingStatus.NewCourse;

        var result = courseFundingStatus.GetCourseFundingStatusTagClass();

        result.Should().Be("govuk-tag--yellow");
    }

    [Test]
    public void WhenIsLastDateStartsInFuture_AndDateIsToday_ThenReturnsTrue()
    {
        var result = CourseFundingStatusHelper.IsLastDateStartsInFuture(DateTime.UtcNow.Date);

        result.Should().BeTrue();
    }

    [Test]
    public void WhenIsLastDateStartsInFuture_AndDateIsInThePast_ThenReturnsFalse()
    {
        var result = CourseFundingStatusHelper.IsLastDateStartsInFuture(DateTime.UtcNow.Date.AddDays(-1));

        result.Should().BeFalse();
    }

    [Test]
    public void WhenFormattingDate_ThenReturnsDayMonthYearFormat()
    {
        using (new AssertionScope())
        {
            CourseFundingStatusHelper.FormatDate(new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc))
                .Should().Be("16 Jul 2027");
            CourseFundingStatusHelper.FormatDate(new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc))
                .Should().Be("27 Mar 2026");
        }
    }
}
