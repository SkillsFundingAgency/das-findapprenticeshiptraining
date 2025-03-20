using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api;

public class CreateShortlistForUserRequest : IPostApiRequest<PostShortlistForUserRequest>
{
    public CreateShortlistForUserRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string PostUrl => $"{BaseUrl}shortlists";
    public PostShortlistForUserRequest Data { get; set; }
}

public class PostShortlistForUserRequest
{
    public Guid ShortlistUserId { get; set; }
    public int LarsCode { get; set; }
    public int Ukprn { get; set; }
    public string LocationName { get; set; } = null;
}
