using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class TrainingCourse
    {
        [JsonProperty("trainingCourse")]
        public Course Course { get; set; }
    }
}