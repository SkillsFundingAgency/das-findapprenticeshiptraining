using System;
using System.Globalization;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Services;

public enum CourseFundingStatus
{
    NewCourse,
    ClosingToNewLearnersSoon,
    ClosedToNewLearners
}

public static class CourseFundingStatusHelper
{
    public const string NewCourseText = "New course";
    public const string ClosingToNewLearnersSoonText = "Closing to new learners soon";
    public const string ClosedToNewLearnersText = "Closed to new learners";
    public const string NewCourseDescriptionText = "This is a new course. It isn't accepting learners yet. Contact a training provider to find out when it may start.";
    public const string ClosingToNewLearnersSoonDescriptionText = "This course is closing on {0}. Government funding will not be available for learners who start after this date.";
    public const string ClosedToNewLearnersDescriptionText = "This course stopped accepting new learners on {0}. Apprentices who are already on this course can still change training providers.";
    public const string ProviderCourseAddedDescriptionText = "This course was added by the training provider on {0}.";
    public const string ProviderCourseClosingDescriptionText = "This training provider will stop offering this course on {0}";
    public const string ProviderCourseDetailsClosingDescriptionText = "This training provider will stop offering this course to new learners on {0}";

    public static CourseFundingStatus? GetCourseStatusTag(StandardDates courseDates, DateTime currentDate)
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
            return CourseFundingStatus.NewCourse;
        }

        if (lastDateStarts < today)
        {
            return CourseFundingStatus.ClosedToNewLearners;
        }

        return CourseFundingStatus.ClosingToNewLearnersSoon;
    }

    public static string GetCourseStatusTagText(CourseFundingStatus? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseFundingStatus.NewCourse => NewCourseText,
            CourseFundingStatus.ClosingToNewLearnersSoon => ClosingToNewLearnersSoonText,
            CourseFundingStatus.ClosedToNewLearners => ClosedToNewLearnersText,
            _ => string.Empty
        };
    }

    public static string GetCourseStatusTagClass(CourseFundingStatus? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseFundingStatus.NewCourse => "govuk-tag--yellow",
            CourseFundingStatus.ClosingToNewLearnersSoon => "govuk-tag--orange",
            CourseFundingStatus.ClosedToNewLearners => "govuk-tag--grey",
            _ => string.Empty
        };
    }

    public static string GetCourseStatusTagDescription(CourseFundingStatus? courseStatusTag, DateTime? lastDateStarts)
    {
        return courseStatusTag switch
        {
            CourseFundingStatus.NewCourse => NewCourseDescriptionText,
            CourseFundingStatus.ClosingToNewLearnersSoon when lastDateStarts.HasValue => string.Format(CultureInfo.InvariantCulture, ClosingToNewLearnersSoonDescriptionText, FormatDate(lastDateStarts.Value)),
            CourseFundingStatus.ClosedToNewLearners when lastDateStarts.HasValue => string.Format(CultureInfo.InvariantCulture, ClosedToNewLearnersDescriptionText, FormatDate(lastDateStarts.Value)),
            _ => string.Empty
        };
    }

    public static string GetProviderCourseAddedDescription(DateTime providerCourseAddedDate)
    {
        return string.Format(CultureInfo.InvariantCulture, ProviderCourseAddedDescriptionText, FormatDate(providerCourseAddedDate));
    }

    public static string GetProviderCourseClosingDescription(DateTime? lastDateStarts, DateTime currentDate)
    {
        return HasProviderCourseClosingDate(lastDateStarts, currentDate)
            ? string.Format(CultureInfo.InvariantCulture, ProviderCourseClosingDescriptionText, FormatDate(lastDateStarts.Value))
            : string.Empty;
    }

    public static bool HasProviderCourseClosingDate(DateTime? lastDateStarts, DateTime currentDate)
    {
        return lastDateStarts.HasValue && lastDateStarts.Value.Date >= currentDate.Date;
    }

    public static DateTime? GetProviderCourseDetailsClosingDate(DateTime? providerLastDateStarts, StandardDates courseDates)
    {
        if (providerLastDateStarts.HasValue)
        {
            return providerLastDateStarts;
        }

        if (courseDates?.LastDateStarts == null)
        {
            return null;
        }

        if (courseDates.LastDateStarts.Value.Date == courseDates.EffectiveFrom.Date)
        {
            return null;
        }

        return courseDates.LastDateStarts;
    }

    public static string GetProviderCourseDetailsClosingDescription(DateTime? providerLastDateStarts, StandardDates courseDates, DateTime currentDate)
    {
        var closingDate = GetProviderCourseDetailsClosingDate(providerLastDateStarts, courseDates);

        return HasProviderCourseClosingDate(closingDate, currentDate)
            ? string.Format(CultureInfo.InvariantCulture, ProviderCourseDetailsClosingDescriptionText, FormatDate(closingDate.Value))
            : string.Empty;
    }

    public static string FormatDate(DateTime date)
    {
        return date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
    }
}
