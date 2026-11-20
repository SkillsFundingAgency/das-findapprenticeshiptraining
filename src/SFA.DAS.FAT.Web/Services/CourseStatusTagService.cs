using System;
using System.Globalization;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Services;

public enum CourseStatusTag
{
    NewCourse,
    ClosingToNewLearnersSoon,
    ClosedToNewLearners
}

public static class CourseStatusTagService
{
    public const string NewCourseText = "New course";
    public const string ClosingToNewLearnersSoonText = "Closing to new learners soon";
    public const string ClosedToNewLearnersText = "Closed to new learners";
    public const string NewCourseDescriptionText = "This is a new course. It isn't accepting learners yet. Contact a training provider to find out when it may start.";
    public const string ClosingToNewLearnersSoonDescriptionText = "This course is closing on {0}. Government funding will not be available for learners who start after this date.";
    public const string ClosedToNewLearnersDescriptionText = "This course stopped accepting new learners on {0}. Apprentices who are already on this course can still change training providers.";
    public const string ProviderCourseAddedDescriptionText = "This course was added by the training provider on {0}.";
    public const string ProviderCourseClosingDescriptionText = "This training provider will stop offering this course on {0}";

    public static CourseStatusTag? GetCourseStatusTag(StandardDates courseDates, DateTime currentDate)
    {
        if (courseDates?.LastDateStarts == null)
        {
            return null;
        }

        var lastDateStarts = courseDates.LastDateStarts.Value.Date;
        var effectiveFrom = courseDates.EffectiveFrom.Date;
        var today = currentDate.Date;

        if (lastDateStarts == effectiveFrom)
        {
            return CourseStatusTag.NewCourse;
        }

        if (lastDateStarts < today)
        {
            return CourseStatusTag.ClosedToNewLearners;
        }

        return CourseStatusTag.ClosingToNewLearnersSoon;
    }

    public static string GetCourseStatusTagText(CourseStatusTag? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseStatusTag.NewCourse => NewCourseText,
            CourseStatusTag.ClosingToNewLearnersSoon => ClosingToNewLearnersSoonText,
            CourseStatusTag.ClosedToNewLearners => ClosedToNewLearnersText,
            _ => string.Empty
        };
    }

    public static string GetCourseStatusTagClass(CourseStatusTag? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseStatusTag.NewCourse => "govuk-tag--yellow",
            CourseStatusTag.ClosingToNewLearnersSoon => "govuk-tag--orange",
            CourseStatusTag.ClosedToNewLearners => "govuk-tag--grey",
            _ => string.Empty
        };
    }

    public static string GetCourseStatusTagDescription(CourseStatusTag? courseStatusTag, DateTime? lastDateStarts)
    {
        return courseStatusTag switch
        {
            CourseStatusTag.NewCourse => NewCourseDescriptionText,
            CourseStatusTag.ClosingToNewLearnersSoon when lastDateStarts.HasValue => string.Format(CultureInfo.InvariantCulture, ClosingToNewLearnersSoonDescriptionText, FormatDate(lastDateStarts.Value)),
            CourseStatusTag.ClosedToNewLearners when lastDateStarts.HasValue => string.Format(CultureInfo.InvariantCulture, ClosedToNewLearnersDescriptionText, FormatDate(lastDateStarts.Value)),
            _ => string.Empty
        };
    }

    public static string GetProviderCourseAddedDescription(DateTime providerCourseAddedDate)
    {
        return string.Format(CultureInfo.InvariantCulture, ProviderCourseAddedDescriptionText, FormatDate(providerCourseAddedDate));
    }

    public static bool HasProviderCourseClosingDate(DateTime? lastDateStarts, DateTime currentDate)
    {
        return lastDateStarts.HasValue && lastDateStarts.Value.Date >= currentDate.Date;
    }

    public static string GetProviderCourseClosingDescription(DateTime? lastDateStarts, DateTime currentDate)
    {
        return HasProviderCourseClosingDate(lastDateStarts, currentDate)
            ? string.Format(CultureInfo.InvariantCulture, ProviderCourseClosingDescriptionText, FormatDate(lastDateStarts.Value))
            : string.Empty;
    }

    public static string FormatDate(DateTime date)
    {
        return date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
    }
}
