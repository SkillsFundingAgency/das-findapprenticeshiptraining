using SFA.DAS.FAT.Domain.CourseProviders;

namespace SFA.DAS.FAT.Web.Models;

public class ProviderOrderByOptionViewModel
{
    public bool Selected { get; set; }
    public string Description { get; set; }
    public ProviderOrderBy ProviderOrderBy { get; set; }
}
