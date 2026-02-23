namespace SFA.DAS.FAT.Web.Models.BreadCrumbs;

public class PageLinksViewModelBase
{
    public bool ShowSearchCrumb { get; set; } = true;
    public bool ShowApprenticeTrainingCoursesCrumb { get; set; }
    public bool ShowApprenticeTrainingCourseCrumb { get; set; }
    public bool ShowApprenticeTrainingCourseProvidersCrumb { get; set; }
    public string Location { get; set; }
    public string Distance { get; set; }
    public string LarsCode { get; set; }
    public bool ShowShortListLink { get; set; }
}
