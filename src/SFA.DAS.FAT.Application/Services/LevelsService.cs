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
public sealed class LevelsService(
    IApiClient _apiClient,
    ISessionService _sessionService,
    IDistributedCacheService _distributedCacheService,
    IOptions<FindApprenticeshipTrainingApi> config
) : ILevelsService
{
    public async Task<IEnumerable<Level>> GetLevelsAsync(CancellationToken cancellationToken)
    {
        var sessionLevels = _sessionService.Get<List<Level>>(SessionKeys.StandardLevels);
        if (sessionLevels?.Count > 0)
        {
            return sessionLevels;
        }

        var cachedLevels = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.Levels.Key,
            async () =>
            {
                var apiResponse = await _apiClient.Get<GetLevelsListResponse>(
                    new GetCourseLevelsApiRequest(config.Value.BaseUrl)
                );

                if (apiResponse?.Levels?.Any() != true)
                {

                    throw new InvalidOperationException("Could not retrieve course levels from any source.");
                }

                return apiResponse.Levels;
            },
            CacheSetting.Levels.CacheDuration
        );

        _sessionService.Set(SessionKeys.StandardLevels, cachedLevels);
        return cachedLevels;
    }
}
