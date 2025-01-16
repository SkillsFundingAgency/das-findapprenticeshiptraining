#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Domain.Providers.Api;

namespace SFA.DAS.FAT.Application.Providers.Services;
public class ProviderService(IApiClient outerApiClient, IOptions<FindApprenticeshipTrainingApi> config) : IProviderService
{
    public async Task<IEnumerable<RegisteredProvider>> GetRegisteredProviders(string? searchTerm)
    {
        searchTerm ??= string.Empty;
        searchTerm = searchTerm.Trim();
        if (searchTerm.Length < 3) return new List<RegisteredProvider>();

        List<RegisteredProvider> providers = (await GetProviders(config.Value.BaseUrl))!;
        var matchedProviders = providers
            .Where(provider => provider.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                               || provider.Ukprn.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        return matchedProviders;

    }

    private async Task<List<RegisteredProvider>?> GetProviders(string baseUrl)
    {
        //Look in session cache for providers, and return details, else...

        var request = new GetRegisteredProvidersApiRequest(baseUrl);

        var result = await outerApiClient.Get<RegisteredProviders>(request);

        // save to session cache

        return result.Providers;
    }
}
