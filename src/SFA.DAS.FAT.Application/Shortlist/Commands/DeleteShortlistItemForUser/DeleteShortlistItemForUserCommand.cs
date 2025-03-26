using System;
using MediatR;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;

public class DeleteShortlistItemForUserCommand : IRequest
{
    public Guid Id { get; set; }
}
