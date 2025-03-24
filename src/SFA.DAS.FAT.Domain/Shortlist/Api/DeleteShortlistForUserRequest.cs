using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api;

public class DeleteShortlistForUserRequest : IDeleteApiRequest
{
    private readonly Guid _id;

    public DeleteShortlistForUserRequest(string baseUrl, Guid shortlistId)
    {
        _id = shortlistId;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string DeleteUrl => $"{BaseUrl}shortlists/{_id}";
}
