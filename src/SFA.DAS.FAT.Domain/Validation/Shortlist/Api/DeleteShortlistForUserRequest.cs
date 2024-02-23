using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Shortlist.Api
{
    public class DeleteShortlistForUserRequest : IDeleteApiRequest
    {
        private readonly Guid _id;
        private readonly Guid _shortlistUserId;

        public DeleteShortlistForUserRequest(string baseUrl, Guid id, Guid shortlistUserId)
        {
            _id = id;
            _shortlistUserId = shortlistUserId;
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public string DeleteUrl => $"{BaseUrl}shortlist/users/{_shortlistUserId}/items/{_id}";
    }
}