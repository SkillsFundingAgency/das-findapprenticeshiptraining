using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class ProviderReviewsViewModel
{
    public ReviewsModel Reviews { get; set; }
    public string EmployerReviewsDisplayMessage { get; set; }
    public string ApprenticeReviewsDisplayMessage { get; set; }
}
