using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class Level
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
