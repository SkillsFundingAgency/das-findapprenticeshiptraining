using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.CourseProviders;
public enum ProviderOrderBy
{
    [Description("Distance")]
    Distance,
    [Description("Achievement rate")]
    AchievementRate,
    [Description("Employer reviews")]
    EmployerProviderRating,
    [Description("Apprentice reviews")]
    ApprenticeProviderRating
}
