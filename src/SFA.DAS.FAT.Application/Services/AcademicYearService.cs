using System;
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
        var sessionData = _sessionService.Get<GetAcademicYearsLatestResponse>();
        if (!string.IsNullOrEmpty(sessionData?.QarPeriod) && !string.IsNullOrEmpty(sessionData?.ReviewPeriod))
        {
            return sessionData;
        }

        var cachedData = await _distributedCacheService.GetAsync<GetAcademicYearsLatestResponse>(
            CacheSetting.AcademicYearsLatest.Key
        );
        if (!string.IsNullOrEmpty(cachedData?.QarPeriod) && !string.IsNullOrEmpty(cachedData?.ReviewPeriod))
        {
            _sessionService.Set(cachedData);
            return cachedData;
        }

        var apiResponse = await _apiClient.Get<GetAcademicYearsLatestResponse>(
            new GetAcademicYearsLatestRequest(config.Value.BaseUrl)
        );

        if (!string.IsNullOrEmpty(apiResponse?.QarPeriod) && !string.IsNullOrEmpty(apiResponse?.ReviewPeriod))
        {
            await _distributedCacheService.SetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                apiResponse,
                CacheSetting.AcademicYearsLatest.CacheDuration
            );

            _sessionService.Set(apiResponse);
            return apiResponse;
        }

        throw new InvalidOperationException("Could not retrieve academic years from any source.");
    }
}
