using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain;
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
        var sessionData = _sessionService.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears);
        if (sessionData?.QarPeriod != null && sessionData.ReviewPeriod != null)
        {
            return sessionData;
        }

        var result = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            async () =>
            {
                var apiResponse = await _apiClient.Get<GetAcademicYearsLatestResponse>(
                    new GetAcademicYearsLatestRequest(config.Value.BaseUrl)
                );

                if (apiResponse?.QarPeriod != null && apiResponse.ReviewPeriod != null)
                {
                    return apiResponse;
                }

                throw new InvalidOperationException("Could not retrieve academic years from any source.");
            },
            CacheSetting.AcademicYearsLatest.CacheDuration
        );

        _sessionService.Set(SessionKeys.AcademicYears, result);
        return result;
    }
}
