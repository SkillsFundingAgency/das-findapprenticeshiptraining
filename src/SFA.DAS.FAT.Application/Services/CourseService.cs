using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.CourseProviders.Api;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Services;

public class CourseService : ICourseService
{
    private readonly IApiClient _apiClient;
    private readonly FindApprenticeshipTrainingApi _config;

    public CourseService(IApiClient apiClient, IOptions<FindApprenticeshipTrainingApi> config)
    {
        _apiClient = apiClient;
        _config = config.Value;
    }

    public async Task<GetCourseResponse> GetCourse(int larsCode, string location, int? distance)
    {
        var request = new GetCourseApiRequest(_config.BaseUrl, larsCode, location, distance);

        return await _apiClient.Get<GetCourseResponse>(request);
    }

    public async Task<CourseProvidersDetails> GetCourseProviders(CourseProvidersParameters courseProvidersParameters)
    {
        var request = new CourseProvidersApiRequest(_config.BaseUrl, courseProvidersParameters);

        var response = await _apiClient.Get<CourseProvidersDetails>(request);

        return response;
    }

    public async Task<CourseProviderDetailsModel> GetCourseProvider(int ukprn, int larsCode, string location, int? distance, Guid shortlistUserId)
    {
        CourseProviderDetailsModel response = null;

        try
        {
            response = await _apiClient.Get<CourseProviderDetailsModel>(
                new GetCourseProviderDetailsApiRequest(
                    _config.BaseUrl,
                    larsCode,
                    ukprn,
                    location,
                    distance,
                    shortlistUserId
                )
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            response = null;
        }

        return response;
    }
}
