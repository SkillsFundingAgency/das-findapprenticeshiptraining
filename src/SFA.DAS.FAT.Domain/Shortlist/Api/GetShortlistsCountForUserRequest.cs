using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api;

public class GetShortlistsCountForUserRequest : IGetApiRequest
{
    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}shortlists/users/{ShortlistUserId}/count";
    public Guid ShortlistUserId { get; }
    public GetShortlistsCountForUserRequest(string baseUrl, Guid shortlistUserId)
    {
        BaseUrl = baseUrl;
        ShortlistUserId = shortlistUserId;
    }
}
