using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;

namespace SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;

public class GetShortlistsForUserQueryHandler : IRequestHandler<GetShortlistsForUserQuery, GetShortlistsForUserResponse>
{
    private readonly IShortlistService _shortlistService;

    public GetShortlistsForUserQueryHandler(IShortlistService shortlistService)
    {
        _shortlistService = shortlistService;
    }

    public async Task<GetShortlistsForUserResponse> Handle(GetShortlistsForUserQuery request, CancellationToken cancellationToken)
    {
        GetShortlistsForUserResponse result = await _shortlistService.GetShortlistsForUser(request.ShortlistUserId);

        return result;
    }
}
