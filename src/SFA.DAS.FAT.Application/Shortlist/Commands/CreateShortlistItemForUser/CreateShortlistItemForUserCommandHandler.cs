using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;

public class CreateShortlistItemForUserCommandHandler : IRequestHandler<CreateShortlistItemForUserCommand, Guid>
{
    private readonly IShortlistService _service;

    public CreateShortlistItemForUserCommandHandler(IShortlistService service)
    {
        _service = service;
    }

    public async Task<Guid> Handle(CreateShortlistItemForUserCommand request, CancellationToken cancellationToken)
    {
        var shortlistId = await _service.CreateShortlistItemForUser(request);

        return shortlistId;
    }
}
