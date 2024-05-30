using SFA.DAS.FAT.Domain.Configuration;

namespace SFA.DAS.FAT.Web.Models
{
    public interface IGetHelpFindingCourseViewModel
    {
        string TitleAndLevel { get; }
        bool CanGetHelpFindingCourse(FindApprenticeshipTrainingWeb config);
        string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config);
    }
}
