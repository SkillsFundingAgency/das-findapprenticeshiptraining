using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.CourseProviders;
public enum ProviderOrderBy
{
    [Description("Distance")]
    Distance,
    [Description("Achievement rate")]
    AchievementRate,
    [Description("Employer provider rating")]
    EmployerProviderRating,
    [Description("Apprentice provider rating")]
    ApprenticeProviderRating
}
