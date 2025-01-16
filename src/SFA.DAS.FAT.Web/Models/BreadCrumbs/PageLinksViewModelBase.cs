namespace SFA.DAS.FAT.Web.Models.BreadCrumbs;

public class PageLinksViewModelBase
{
    public bool ShowHomeCrumb { get; set; } = true;
    public bool ShowApprenticeTrainingCoursesCrumb { get; set; }
    public string Location { get; set; }
    public int CourseId { get; set; }
    public bool ShowApprenticeTrainingCourseCrumb { get; set; }
    public string ApprenticeCourseTitle { get; set; }
    public bool ShowApprenticeTrainingCourseProvidersCrumb { get; set; }
    public int ShortListItemCount { get; set; }

    public bool ShowShortListLink { get; set; }

    public string BackLinkUrl { get; set; }
    public bool ShowBackLink { get; set; } = false;
}
