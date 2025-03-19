using System;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IShortlistService
{
    Task<int> GetShortlistsCountForUser(Guid shortlistUserId);
    Task<ShortlistForUser> GetShortlistForUser(Guid shortlistUserId);
    Task DeleteShortlistItemForUser(Guid id, Guid shortlistUserId);
    Task<Guid> CreateShortlistItemForUser(Guid shortlistUserId, int ukprn, int trainingCode, double? lat, double? lon, string locationDescription);
    Task<Guid> CreateShortlistItemForUser(PostShortlistForUserRequest request);
}
