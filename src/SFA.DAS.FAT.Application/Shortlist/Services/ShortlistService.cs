﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain;
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
        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);
        if (shortlistCount == null)
        {
            var apiRequest = new GetShortlistsCountForUserRequest(_configValue.BaseUrl, shortlistUserId);
            shortlistCount = await _apiClient.Get<ShortlistsCount>(apiRequest);
            _sessionService.Set(SessionKeys.ShortlistCount, shortlistCount);
        }
        return shortlistCount.Count;
    }

    public async Task<GetShortlistsForUserResponse> GetShortlistsForUser(Guid shortlistUserId)
    {
        var apiRequest = new GetShortlistsForUserRequest(_configValue.BaseUrl, shortlistUserId);

        var apiResponse = await _apiClient.Get<GetShortlistsForUserResponse>(apiRequest);

        return apiResponse;
    }

    public async Task DeleteShortlistItem(Guid id)
    {
        var response = await _apiClient.Delete<DeleteShortlistItemResponse>(new DeleteShortlistItemRequest(_configValue.BaseUrl, id));

        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);
        if (response.Success && shortlistCount != null)
        {
            shortlistCount.Count--;
            _sessionService.Set(SessionKeys.ShortlistCount, shortlistCount);
        }
    }

    public async Task<Guid> CreateShortlistItemForUser(PostShortlistForUserRequest request)
    {
        var response = await _apiClient.Post<CreateShortlistForUserResponse, PostShortlistForUserRequest>(new CreateShortlistForUserRequest(_configValue.BaseUrl) { Data = request });

        if (response.IsCreated)
        {
            var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);
            if (shortlistCount != null)
            {
                shortlistCount.Count += 1;
                _sessionService.Set(SessionKeys.ShortlistCount, shortlistCount);
            }
        }

        return response.ShortlistId;
    }
}
