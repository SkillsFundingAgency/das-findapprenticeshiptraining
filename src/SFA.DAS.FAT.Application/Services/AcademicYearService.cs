using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Requests;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Responses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Services;
public sealed class AcademicYearService(
    IApiClient _apiClient,
    ISessionService _sessionService,
    IDistributedCacheService _distributedCacheService,
    IOptions<FindApprenticeshipTrainingApi> config) : IAcademicYearsService
{
    public async Task<GetAcademicYearsLatestResponse> GetAcademicYearsLatestAsync(CancellationToken cancellationToken)
    {

        if (_sessionService.Contains<GetAcademicYearsLatestResponse>())
        {
            return _sessionService.Get<GetAcademicYearsLatestResponse>();
        }

        var academicYearsLatest = await _distributedCacheService.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                () => _apiClient.Get<GetAcademicYearsLatestResponse>(new GetAcademicYearsLatestRequest(config.Value.BaseUrl)),
                CacheSetting.AcademicYearsLatest.CacheDuration
            );

        _sessionService.Set(academicYearsLatest);

        return academicYearsLatest;
    }
}
