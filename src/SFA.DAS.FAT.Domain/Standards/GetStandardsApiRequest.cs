using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Standards;

public class GetStandardsApiRequest : IGetApiRequest
{
    public string BaseUrl { get; }
    public GetStandardsApiRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }
    public string GetUrl => $"{BaseUrl}standards";
}
