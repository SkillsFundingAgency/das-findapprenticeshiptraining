using System;
using MediatR;
using SFA.DAS.FAT.Domain.Shortlist;

namespace SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;

public record GetShortlistsForUserQuery(Guid ShortlistUserId) : IRequest<GetShortlistsForUserResponse>;
