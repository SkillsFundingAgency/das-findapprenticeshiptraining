using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class AchievementsAndParticipationViewModel
{
    public QarModel Qar { get; set; }
    public string AchievementRateInformation { get; set; }
    public string EndpointAssessmentsCountDisplay { get; set; }
    public string EndpointAssessmentDisplayMessage { get; set; }
}
