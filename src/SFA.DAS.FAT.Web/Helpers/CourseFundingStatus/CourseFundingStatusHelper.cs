using System;
using System.Globalization;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Helpers;

public static class CourseFundingStatusHelper
{
    public static CourseFundingStatus? GetCourseFundingStatus(StandardDates courseDates, DateTime currentDate)
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

    public static bool IsLastDateStartsInFuture(DateTime? lastDateStarts)
    {
        return lastDateStarts.HasValue && lastDateStarts.Value.Date >= DateTime.UtcNow.Date;
    }

    public static string FormatDate(DateTime date)
    {
        return date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
    }
}
