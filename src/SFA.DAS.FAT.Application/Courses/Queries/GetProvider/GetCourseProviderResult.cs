using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetProvider
{
    public class GetCourseProviderResult
    {
        public Provider Provider { get; set; }
        public Course Course { get ; set ; }
        public AdditionalCourses AdditionalCourses { get; set; }
        public string Location { get ; set ; }
        public List<double> LocationGeoPoint { get ; set ; }
        public int ProvidersAtLocation { get ; set ; }
        public int ShortlistItemCount { get ; set ; }
        public int TotalProviders { get ; set ; }
        public bool ShowEmployerDemand { get ; set ; }
    }
}
