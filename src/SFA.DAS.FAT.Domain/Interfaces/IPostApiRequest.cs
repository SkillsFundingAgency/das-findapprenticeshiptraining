using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IPostApiRequest<TData>
{
    [JsonIgnore]
    string PostUrl { get; }

    TData Data { get; set; }
}
