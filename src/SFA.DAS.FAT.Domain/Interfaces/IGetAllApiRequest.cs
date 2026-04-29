using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IGetAllApiRequest
{
    [JsonIgnore]
    string GetAllUrl { get; }
}
