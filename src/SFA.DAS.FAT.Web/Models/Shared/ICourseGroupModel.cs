using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models.Shared;

public interface ICourseGroupModel
{
    int Ukprn { get; }
    string LocationName { get; }
    ProviderCoursesModel ProviderCoursesDetails { get; }
}
