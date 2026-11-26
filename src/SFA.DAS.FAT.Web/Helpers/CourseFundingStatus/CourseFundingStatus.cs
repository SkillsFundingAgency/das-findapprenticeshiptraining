using System.ComponentModel;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Web.Helpers;

public enum CourseFundingStatus
{
    [Description("New course")]
    NewCourse,
    [Description("Closing to new learners soon")]
    ClosingToNewLearnersSoon,
    [Description("Closed to new learners")]
    ClosedToNewLearners
}

public static class CourseFundingStatusExtensions
{
    public static string GetCourseFundingStatusDescription(this CourseFundingStatus? courseFundingStatus)
    {
        return courseFundingStatus.HasValue
            ? courseFundingStatus.Value.GetDescription()
            : string.Empty;
    }

    public static string GetCourseFundingStatusTagClass(this CourseFundingStatus? courseFundingStatus)
    {
        return courseFundingStatus switch
        {
            CourseFundingStatus.NewCourse => "govuk-tag--yellow",
            CourseFundingStatus.ClosingToNewLearnersSoon => "govuk-tag--orange",
            CourseFundingStatus.ClosedToNewLearners => "govuk-tag--grey",
            _ => string.Empty
        };
    }
}
