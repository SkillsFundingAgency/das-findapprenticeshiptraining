using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Providers.Api;
public class GetRegisteredProvidersApiRequest : IGetApiRequest
{
    public GetRegisteredProvidersApiRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}providers";
}
