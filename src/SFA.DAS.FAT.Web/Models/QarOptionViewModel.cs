using SFA.DAS.FAT.Domain.CourseProviders;

namespace SFA.DAS.FAT.Web.Models;

public class QarOptionViewModel
{
    public bool Selected { get; set; }
    public string Description { get; set; }
    public QarRating QarRatingType { get; set; }
}
