using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProviderSubmitModel
{
    public string Location { get; set; }
    public string Distance { get; set; } = DistanceService.TenMiles.ToString();
}
