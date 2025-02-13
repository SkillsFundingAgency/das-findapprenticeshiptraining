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

public sealed class LevelsService(
    IApiClient _apiClient, 
    ISessionService _sessionService, 
    IDistributedCacheService _distributedCacheService,
    IOptions<FindApprenticeshipTrainingApi> config
) : ILevelsService
{
    public async Task<IEnumerable<Level>> GetLevelsAsync(CancellationToken cancellationToken)
    {
        if(_sessionService.Contains<GetLevelsListResponse>())
        {
            var sessionLevels = _sessionService.Get<GetLevelsListResponse>();
            return sessionLevels.Levels;
        }

        var levelsResponse = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.Levels.Key,
            () => _apiClient.Get<GetLevelsListResponse>(new GetCourseLevelsApiRequest(config.Value.BaseUrl)),
            CacheSetting.Levels.CacheDuration
        );

        _sessionService.Set(levelsResponse);

        return levelsResponse.Levels;
    }
}
