using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IDeleteApiRequest
{
    [JsonIgnore]
    string DeleteUrl { get; }
}
