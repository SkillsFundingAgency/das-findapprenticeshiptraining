using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses;

public class ApprenticeType
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}
