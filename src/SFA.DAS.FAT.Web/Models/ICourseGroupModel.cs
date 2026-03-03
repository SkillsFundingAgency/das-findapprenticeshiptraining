namespace SFA.DAS.FAT.Web.Models;

public interface ICourseGroupModel
{
    int Ukprn { get; }
    string Location { get; }
    ProviderCoursesModel ProviderCoursesDetails { get; }
}
