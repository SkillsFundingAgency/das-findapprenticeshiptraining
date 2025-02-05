using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class TrainingCourses
    {
        [JsonProperty("trainingCourses")]
        public List<Course> Courses { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("totalFiltered")]
        public int TotalFiltered { get; set; }
        [JsonProperty("sectors")]
        public List<Route> Sectors { get; set; }
        [JsonProperty("levels")]
        public List<Level> Levels { get; set; }
        [JsonProperty("shortlistItemCount")]
        public int ShortlistItemCount { get; set; }
    }
}