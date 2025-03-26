#nullable enable
using SFA.DAS.FAT.Web.Models.BreadCrumbs;

namespace SFA.DAS.FAT.Web.Models;

public class SelectTrainingProviderViewModel : SelectTrainingProviderSubmitViewModel
{
    public SelectTrainingProviderViewModel()
    {
        ShowSearchCrumb = true;
        ShowShortListLink = true;
    }
}

public class SelectTrainingProviderSubmitViewModel : PageLinksViewModelBase
{
    public string? SearchTerm
    {
        get => string.IsNullOrWhiteSpace(Ukprn) ? string.Empty : $"{Name}: ({Ukprn})";
    }

    public string? Name { get; set; }
    public string? Ukprn { get; set; }
}
