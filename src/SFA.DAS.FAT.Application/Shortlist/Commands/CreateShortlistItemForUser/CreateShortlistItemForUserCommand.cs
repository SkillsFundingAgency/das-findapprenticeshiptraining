using System;
using MediatR;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;

public class CreateShortlistItemForUserCommand : PostShortlistForUserRequest, IRequest<Guid>
{
}
