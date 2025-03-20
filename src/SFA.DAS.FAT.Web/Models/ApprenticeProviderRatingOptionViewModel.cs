using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class ApprenticeProviderRatingOptionViewModel
    {
        public bool Selected { get; set; }
        public string Description { get; set; }
        public ProviderRating ProviderRatingType { get; set; }
    }
}
