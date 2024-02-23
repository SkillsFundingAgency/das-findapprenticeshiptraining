using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api
{
    public class GetShortlistForUserApiRequest : IGetApiRequest
    {
        private readonly Guid _shortlistUserId;

        public GetShortlistForUserApiRequest(string baseUrl, Guid shortlistUserId)
        {
            _shortlistUserId = shortlistUserId;
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}shortlist/users/{_shortlistUserId}";
    }
}
