using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    //MFCMFC this might be redundant now
    public class TrainingCourseProviders
    {
        [JsonProperty("trainingCourse")]
        public Course Course { get; set; }
        [JsonProperty("trainingCourseProviders")]
        public IEnumerable<Provider> CourseProviders { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("totalFiltered")]
        public int TotalFiltered { get; set; }
        [JsonProperty("location")]
        public Locations.Locations.LocationItem Location { get; set; }
        [JsonProperty("shortlistItemCount")]
        public int ShortlistItemCount { get; set; }

        public bool ShowEmployerDemand { get; set; }
    }
}
