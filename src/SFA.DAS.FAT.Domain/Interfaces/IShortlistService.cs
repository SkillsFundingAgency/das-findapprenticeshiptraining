using System;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IShortlistService
{
    Task<int> GetShortlistsCountForUser(Guid shortlistUserId);
    Task<GetShortlistsForUserResponse> GetShortlistsForUser(Guid shortlistUserId);
    Task DeleteShortlistItemForUser(Guid id);
    Task<Guid> CreateShortlistItemForUser(PostShortlistForUserRequest request);
}
