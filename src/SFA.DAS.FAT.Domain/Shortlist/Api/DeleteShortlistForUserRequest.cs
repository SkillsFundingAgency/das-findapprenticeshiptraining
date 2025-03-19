using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api;

public class DeleteShortlistForUserRequest : IDeleteApiRequest
{
    private readonly Guid _id;
    private readonly Guid _shortlistUserId;

    [Obsolete]
    public DeleteShortlistForUserRequest(string baseUrl, Guid id, Guid shortlistUserId)
    {
        _id = id;
        _shortlistUserId = shortlistUserId;
        BaseUrl = baseUrl;
    }
    public DeleteShortlistForUserRequest(string baseUrl, Guid shortlistId)
    {
        _id = shortlistId;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string DeleteUrl => $"{BaseUrl}shortlists/{_id}";
}
