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
    private readonly ISessionService _sessionService;
    private readonly FindApprenticeshipTrainingApi _configValue;

    public ShortlistService(IApiClient apiClient, IOptions<FindApprenticeshipTrainingApi> config, ISessionService sessionService)
    {
        _apiClient = apiClient;
        _sessionService = sessionService;
        _configValue = config.Value;
    }

    public async Task<int> GetShortlistsCountForUser(Guid shortlistUserId)
    {
        var shortlistCount = _sessionService.Get<ShortlistsCount>();
        if (shortlistCount == null)
        {
            var apiRequest = new GetShortlistsCountForUserRequest(_configValue.BaseUrl, shortlistUserId);
            shortlistCount = await _apiClient.Get<ShortlistsCount>(apiRequest);
            _sessionService.Set(shortlistCount);
        }
        return shortlistCount.Count;
    }

    public async Task<ShortlistForUser> GetShortlistForUser(Guid shortlistUserId)
    {
        var apiRequest = new GetShortlistForUserApiRequest(_configValue.BaseUrl, shortlistUserId);

        var apiResponse = await _apiClient.Get<ShortlistForUser>(apiRequest);

        return apiResponse;
    }

    public async Task DeleteShortlistItemForUser(Guid id)
    {
        await _apiClient.Delete(new DeleteShortlistForUserRequest(_configValue.BaseUrl, id));

        var shortlistCount = _sessionService.Get<ShortlistsCount>();
        shortlistCount.Count--;
        _sessionService.Set(shortlistCount);

    }

    public async Task<Guid> CreateShortlistItemForUser(PostShortlistForUserRequest request)
    {
        var response = await _apiClient.Post<string, PostShortlistForUserRequest>(new CreateShortlistForUserRequest(_configValue.BaseUrl) { Data = request });

        var shortlistCount = _sessionService.Get<ShortlistsCount>() ?? new ShortlistsCount();
        shortlistCount.Count++;
        _sessionService.Set(shortlistCount);

        return Guid.Parse(response);
    }
}
