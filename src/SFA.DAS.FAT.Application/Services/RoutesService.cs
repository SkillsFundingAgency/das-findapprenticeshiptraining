using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain;
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
        var sessionRoutes = _sessionService.Get<List<Route>>(SessionKeys.StandardRoutes);
        if (sessionRoutes?.Count > 0)
        {
            return sessionRoutes;
        }

        var cachedRoutes = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.Routes.Key,
            async () =>
            {
                var apiResponse = await _apiClient.Get<GetRoutesListResponse>(
                    new GetCourseRoutesApiRequest(config.Value.BaseUrl)
                );

                if (apiResponse?.Routes?.Any() != true)
                {
                    throw new InvalidOperationException("Could not retrieve course routes from any source.");
                }

                return apiResponse.Routes;
            },
            CacheSetting.Routes.CacheDuration
        );

        _sessionService.Set(SessionKeys.StandardRoutes, cachedRoutes);
        return cachedRoutes;
    }
}
