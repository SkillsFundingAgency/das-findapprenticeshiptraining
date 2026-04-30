using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IGetApiRequest
{
    [JsonIgnore]
    string GetUrl { get; }
}
