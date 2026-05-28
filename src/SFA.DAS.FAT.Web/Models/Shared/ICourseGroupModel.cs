using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models.Shared;

public interface ICourseGroupModel
{
    int Ukprn { get; }
    string Location { get; }
    ProviderCoursesModel ProviderCoursesDetails { get; }
}
