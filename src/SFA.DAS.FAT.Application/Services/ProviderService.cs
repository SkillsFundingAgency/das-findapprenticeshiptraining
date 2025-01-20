#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Domain.Providers.Api;

namespace SFA.DAS.FAT.Application.Services;
public class ProviderService(IApiClient outerApiClient, IOptions<FindApprenticeshipTrainingApi> config) : IProviderService
{
    public async Task<List<RegisteredProvider>> GetRegisteredProviders()
    {
        var providers = (await GetProviders(config.Value.BaseUrl))!;
        return providers;
    }

    private async Task<List<RegisteredProvider>?> GetProviders(string baseUrl)
    {
        var request = new GetRegisteredProvidersApiRequest(baseUrl);

        var result = await outerApiClient.Get<RegisteredProviders>(request);

        return result.Providers;
    }
}
