using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses;

public class Route
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}
