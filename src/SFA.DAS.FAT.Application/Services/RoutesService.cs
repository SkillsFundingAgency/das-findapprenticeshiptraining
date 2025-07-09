using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Services;

public sealed class RoutesService(
    IApiClient _apiClient,
    ISessionService _sessionService,
    IDistributedCacheService _distributedCacheService,
    IOptions<FindApprenticeshipTrainingApi> config
) : IRoutesService
{
    public async Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken)
    {
        var sessionRoutes = _sessionService.Get<List<Route>>();
        if (sessionRoutes?.Any() == true)
        {
            return sessionRoutes;
        }

        var cachedRoutes = await _distributedCacheService.GetAsync<List<Route>>(CacheSetting.Routes.Key);
        if (cachedRoutes?.Any() == true)
        {
            _sessionService.Set(cachedRoutes);
            return cachedRoutes;
        }

        var apiResponse = await _apiClient.Get<GetRoutesListResponse>(
            new GetCourseRoutesApiRequest(config.Value.BaseUrl)
        );

        if (apiResponse?.Routes?.Any() == true)
        {
            await _distributedCacheService.SetAsync(
                CacheSetting.Routes.Key,
                apiResponse.Routes,
                CacheSetting.Routes.CacheDuration
            );

            _sessionService.Set(apiResponse.Routes);
            return apiResponse.Routes;
        }

        throw new InvalidOperationException("Could not retrieve course routes from any source.");
    }
}
