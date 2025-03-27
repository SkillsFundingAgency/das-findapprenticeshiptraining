using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api;

public class GetShortlistsForUserRequest : IGetApiRequest
{
    private readonly Guid _shortlistUserId;

    public GetShortlistsForUserRequest(string baseUrl, Guid shortlistUserId)
    {
        _shortlistUserId = shortlistUserId;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}shortlists/users/{_shortlistUserId}";
}
