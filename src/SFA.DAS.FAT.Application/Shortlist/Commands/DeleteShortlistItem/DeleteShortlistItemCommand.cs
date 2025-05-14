using System;
using MediatR;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItem;

public class DeleteShortlistItemCommand : IRequest
{
    public Guid Id { get; set; }
}
