using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class CourseFundingStatusHelperTests
{
    [Test]
    public void WhenGettingTag_AndLastDateStartsMatchesEffectiveFrom_ThenReturnsNewCourseTag()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTag(
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
        var result = CourseFundingStatusHelper.GetCourseStatusTag(
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
        var result = CourseFundingStatusHelper.GetCourseStatusTag(
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
        var result = CourseFundingStatusHelper.GetCourseStatusTag(
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
        var result = CourseFundingStatusHelper.GetCourseStatusTag(
            null,
            new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingTagText_AndTagIsNewCourse_ThenReturnsNewCourseText()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagText(CourseFundingStatus.NewCourse);

        result.Should().Be(CourseFundingStatusHelper.NewCourseText);
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosedToNewLearners_ThenReturnsGreyTagClass()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagClass(CourseFundingStatus.ClosedToNewLearners);

        result.Should().Be("govuk-tag--grey");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsClosingToNewLearnersSoon_ThenReturnsOrangeTagClass()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagClass(CourseFundingStatus.ClosingToNewLearnersSoon);

        result.Should().Be("govuk-tag--orange");
    }

    [Test]
    public void WhenGettingTagClass_AndTagIsNewCourse_ThenReturnsYellowTagClass()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagClass(CourseFundingStatus.NewCourse);

        result.Should().Be("govuk-tag--yellow");
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsNewCourse_ThenReturnsNewCourseDescription()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagDescription(CourseFundingStatus.NewCourse, null);

        result.Should().Be(CourseFundingStatusHelper.NewCourseDescriptionText);
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsClosingToNewLearnersSoon_ThenReturnsClosingDescriptionWithFormattedDate()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagDescription(
            CourseFundingStatus.ClosingToNewLearnersSoon,
            new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course is closing on 16 Jul 2027. Government funding will not be available for learners who start after this date.");
    }

    [Test]
    public void WhenGettingTagDescription_AndTagIsClosedToNewLearners_ThenReturnsClosedDescriptionWithFormattedDate()
    {
        var result = CourseFundingStatusHelper.GetCourseStatusTagDescription(
            CourseFundingStatus.ClosedToNewLearners,
            new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course stopped accepting new learners on 30 Jun 2026. Apprentices who are already on this course can still change training providers.");
    }

    [Test]
    public void WhenGettingProviderCourseAddedDescription_AndDateProvided_ThenReturnsFormattedDescription()
    {
        var result = CourseFundingStatusHelper.GetProviderCourseAddedDescription(
            new DateTime(2026, 6, 13, 0, 0, 0, DateTimeKind.Utc));

        result.Should().Be("This course was added by the training provider on 13 Jun 2026.");
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsInTheFuture_ThenReturnsFormattedDescription()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);
        var result = CourseFundingStatusHelper.GetProviderCourseClosingDescription(
            new DateTime(2027, 3, 27, 0, 0, 0, DateTimeKind.Utc),
            currentDate);

        result.Should().Be("This training provider will stop offering this course on 27 Mar 2027");
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsInThePast_ThenReturnsEmptyString()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);
        var result = CourseFundingStatusHelper.GetProviderCourseClosingDescription(
            new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc),
            currentDate);

        result.Should().BeEmpty();
    }

    [Test]
    public void WhenGettingProviderCourseClosingDescription_AndDateIsNull_ThenReturnsEmptyString()
    {
        var result = CourseFundingStatusHelper.GetProviderCourseClosingDescription(
            null,
            new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc));

        result.Should().BeEmpty();
    }

    [Test]
    public void WhenHasProviderCourseClosingDate_AndDateIsToday_ThenReturnsTrue()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseFundingStatusHelper.HasProviderCourseClosingDate(currentDate, currentDate);

        result.Should().BeTrue();
    }

    [Test]
    public void WhenHasProviderCourseClosingDate_AndDateIsInThePast_ThenReturnsFalse()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseFundingStatusHelper.HasProviderCourseClosingDate(
            currentDate.AddDays(-1),
            currentDate);

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

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndProviderAndCourseDatesExist_ThenReturnsProviderDate()
    {
        var providerDate = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);
        var courseDates = new StandardDates
        {
            LastDateStarts = new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc),
            EffectiveFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(providerDate, courseDates);

        result.Should().Be(providerDate);
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndOnlyProviderDateExists_ThenReturnsProviderDate()
    {
        var providerDate = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(providerDate, null);

        result.Should().Be(providerDate);
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndOnlyCourseDateExists_ThenReturnsCourseDate()
    {
        var courseDate = new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc);
        var courseDates = new StandardDates
        {
            LastDateStarts = courseDate,
            EffectiveFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(null, courseDates);

        result.Should().Be(courseDate);
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndCourseDateMatchesEffectiveFrom_ThenReturnsNull()
    {
        var courseDates = new StandardDates
        {
            LastDateStarts = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc),
            EffectiveFrom = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(null, courseDates);

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndCourseDatesIsNull_ThenReturnsNull()
    {
        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(null, null);

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDate_AndCourseLastDateStartsIsNull_ThenReturnsNull()
    {
        var courseDates = new StandardDates
        {
            LastDateStarts = null,
            EffectiveFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDate(null, courseDates);

        result.Should().BeNull();
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDescription_AndProviderDateIsInTheFuture_ThenReturnsFormattedDescription()
    {
        var currentDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        var providerDate = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDescription(
            providerDate,
            null,
            currentDate);

        result.Should().Be("This training provider will stop offering this course to new learners on 16 Oct 2026");
    }

    [Test]
    public void WhenGettingProviderCourseDetailsClosingDescription_AndClosingDateIsInThePast_ThenReturnsEmptyString()
    {
        var currentDate = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);
        var providerDate = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);

        var result = CourseFundingStatusHelper.GetProviderCourseDetailsClosingDescription(
            providerDate,
            null,
            currentDate);

        result.Should().BeEmpty();
    }
}
