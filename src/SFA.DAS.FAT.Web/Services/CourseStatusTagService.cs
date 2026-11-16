using System;
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

    public static CourseStatusTag? GetTag(StandardDates courseDates, DateTime currentDate)
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

    public static string GetTagText(CourseStatusTag? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseStatusTag.NewCourse => NewCourseText,
            CourseStatusTag.ClosingToNewLearnersSoon => ClosingToNewLearnersSoonText,
            CourseStatusTag.ClosedToNewLearners => ClosedToNewLearnersText,
            _ => string.Empty
        };
    }

    public static string GetTagClass(CourseStatusTag? courseStatusTag)
    {
        return courseStatusTag switch
        {
            CourseStatusTag.NewCourse => "govuk-tag--yellow",
            CourseStatusTag.ClosingToNewLearnersSoon => "govuk-tag--orange",
            CourseStatusTag.ClosedToNewLearners => "govuk-tag--grey",
            _ => string.Empty
        };
    }
}
