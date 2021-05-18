using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Models
{
    public class BreadCrumbViewModel
    {
        public BreadCrumbLevel Level { get; set; }
        public string CourseId { get; set; }
        public string CourseDescription { get; set; }
        public string Location { get; set; }
        public Dictionary<string, string> ProvidersFilters { get; set; }
    }

    public enum BreadCrumbLevel
    {
        Home,
        Error,
        Accessibility,
        CourseList,
        CourseDetails,
        CourseProvidersList,
        CourseProviderDetails
    }
}
