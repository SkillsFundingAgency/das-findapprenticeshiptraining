using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("registeredProviders", Name = RouteNames.GetRegisteredProviders)]
public class RegisteredProvidersController(IDistributedCacheService cacheStorageService, IProviderService providerService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetRegisteredProviders([FromQuery] string query,
        CancellationToken cancellationToken)
    {
        query ??= string.Empty;
        var searchTerm = query.Trim();
        if (searchTerm.Length < 3) return Ok(new List<RegisteredProvider>());

        var providers = await cacheStorageService.GetOrSetAsync(CacheStorageValues.Providers.Key,
            () => providerService.GetRegisteredProviders(),
            TimeSpan.FromHours(CacheStorageValues.Providers.HoursToCache));

        var matchedProviders = providers
            .Where(provider => provider.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                               || provider.Ukprn.ToString().Contains(query, StringComparison.OrdinalIgnoreCase));

        return Ok(matchedProviders.Take(100));
    }
}
