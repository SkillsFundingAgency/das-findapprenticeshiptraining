using System;
using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    [Obsolete("FAT25 remove this interface and set base url in http client setup")]
    public interface IBaseApiRequest
    {
        [JsonIgnore]
        string BaseUrl { get; }
    }
}
