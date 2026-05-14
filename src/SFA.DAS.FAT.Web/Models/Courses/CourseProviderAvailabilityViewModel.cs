namespace SFA.DAS.FAT.Web.Models.Courses;

public class CourseProviderAvailabilityViewModel
{
    public int TotalProvidersCount { get; set; }
    public bool IsShortCourseType { get; set; }
    public string TitleAndLevel { get; set; }
    public string HelpFindingCourseUrl { get; set; }
    public string ProviderCountDisplayMessage { get; set; }
    public bool HasLocation { get; set; }
    public string LarsCode { get; set; }
    public string Location { get; set; }
    public string ApprenticeCanTravelDisplayMessage { get; set; }
    public string Distance { get; set; }
}
