using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Providers.Api.Requests;

public class GetProviderApiRequest : IGetApiRequest
{
    private readonly int _ukprn;

    public GetProviderApiRequest(string baseUrl, int ukprn)
    {
        BaseUrl = baseUrl;
        _ukprn = ukprn;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}providers/{_ukprn}";
}
