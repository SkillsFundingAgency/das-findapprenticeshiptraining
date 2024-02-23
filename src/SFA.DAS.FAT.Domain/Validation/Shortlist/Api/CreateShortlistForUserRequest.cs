using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api
{
    public class CreateShortlistForUserRequest : IPostApiRequest<PostShortlistForUserRequest>
    {
        public CreateShortlistForUserRequest(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public string PostUrl => $"{BaseUrl}shortlist";
        public PostShortlistForUserRequest Data { get; set; }
    }

    public class PostShortlistForUserRequest
    {
        public Guid ShortlistUserId { get; set; }
        public int StandardId { get; set; }
        public int Ukprn { get; set; }
        public double? Lat { get; set; } = null;
        public double? Lon { get; set; } = null;
        public string LocationDescription { get; set; } = null;
    }
}