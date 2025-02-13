using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Services;

public sealed class RoutesService(IApiClient _apiClient, ISessionService _sessionService, IDistributedCacheService _distributedCacheService, IOptions<FindApprenticeshipTrainingApi> config) : IRoutesService
{
    public async Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken)
    {
        if (_sessionService.Contains<GetRoutesListResponse>())
        {
            var sessionRoutes = _sessionService.Get<GetRoutesListResponse>();
            return sessionRoutes.Routes;
        }

        var routesResponse = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.Routes.Key,
            () => _apiClient.Get<GetRoutesListResponse>(new GetCourseRoutesApiRequest(config.Value.BaseUrl)),
            CacheSetting.Routes.CacheDuration
        );

        _sessionService.Set(routesResponse);

        return routesResponse.Routes;
    }
}
