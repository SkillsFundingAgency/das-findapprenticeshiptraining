using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class DeliveryModeOptionViewModel
    {
        public bool Selected { get; set; }
        public string Description { get; set; }
        public ProviderDeliveryMode DeliveryModeChoice { get; set; }
    }
}
