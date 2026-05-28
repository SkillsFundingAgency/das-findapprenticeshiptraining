using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.Shared;

public class AchievementsAndParticipationViewModel
{
    private const string NoDataMessage = "There is not enough data to show the achievement rate for this course. This may be because a small number of apprentices completed this course, or this is a new course for this provider.";

    public QarModel Qar { get; set; }
    public string AchievementRateInformation { get; set; }
    public string EndpointAssessmentsCountDisplay { get; set; }
    public string EndpointAssessmentDisplayMessage { get; set; }

    public bool AchievementRatePresent => Qar?.ConvertedAchievementRate is not null;
    public string AchievementRate => Qar?.AchievementRate ?? string.Empty;
    public string AchievementInformation => AchievementRateInformation ?? string.Empty;
    public string AchievementNoDataMessage => NoDataMessage;
    public bool ShowNationalAchievementRate => AchievementRatePresent;
    public string NationalAchievementRate => Qar?.NationalAchievementRate ?? string.Empty;
    public string ParticipationCount => EndpointAssessmentsCountDisplay ?? string.Empty;
    public string ParticipationMessage => EndpointAssessmentDisplayMessage ?? string.Empty;
}
