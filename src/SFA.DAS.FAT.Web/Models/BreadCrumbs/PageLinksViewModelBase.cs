namespace SFA.DAS.FAT.Web.Models.BreadCrumbs;

public class PageLinksViewModelBase
{
    public bool ShowSearchCrumb { get; set; } = true;
    public bool ShowApprenticeTrainingCoursesCrumb { get; set; }
    public string Location { get; set; }
    public int CourseId { get; set; }
    public bool ShowApprenticeTrainingCourseCrumb { get; set; }

    public bool ShowApprenticeTrainingCourseProvidersCrumb { get; set; }

    public bool ShowShortListLink { get; set; }
}
