using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Application.Shortlist.Services;

public class ShortlistService : IShortlistService
{
    private readonly IApiClient _apiClient;
    private readonly FindApprenticeshipTrainingApi _configValue;

    public ShortlistService(IApiClient apiClient, IOptions<FindApprenticeshipTrainingApi> config)
    {
        _apiClient = apiClient;
        _configValue = config.Value;
    }

    public async Task<int> GetShortlistsCountForUser(Guid shortlistUserId)
    {
        var apiRequest = new GetShortlistsCountForUserRequest(_configValue.BaseUrl, shortlistUserId);
        var apiResponse = await _apiClient.Get<ShortlistsCount>(apiRequest);
        return apiResponse.Count;
    }

    public async Task<ShortlistForUser> GetShortlistForUser(Guid shortlistUserId)
    {
        var apiRequest = new GetShortlistForUserApiRequest(_configValue.BaseUrl, shortlistUserId);

        var apiResponse = await _apiClient.Get<ShortlistForUser>(apiRequest);

        return apiResponse;
    }

    public async Task DeleteShortlistItemForUser(Guid id, Guid shortlistUserId)
    {
        await _apiClient.Delete(new DeleteShortlistForUserRequest(_configValue.BaseUrl, id));
    }

    public async Task<Guid> CreateShortlistItemForUser(Guid shortlistUserId, int ukprn, int trainingCode, double? lat, double? lon, string locationDescription)
    {
        var request = new PostShortlistForUserRequest
        {
            Lat = lat,
            Lon = lon,
            LocationDescription = locationDescription,
            Ukprn = ukprn,
            LarsCode = trainingCode,
            ShortlistUserId = shortlistUserId
        };

        var response = await _apiClient.Post<string, PostShortlistForUserRequest>(new CreateShortlistForUserRequest(_configValue.BaseUrl) { Data = request });

        return Guid.Parse(response);
    }
}
