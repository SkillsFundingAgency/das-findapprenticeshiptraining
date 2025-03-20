using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;

public class DeleteShortlistItemForUserCommandHandler(IShortlistService _service) : IRequestHandler<DeleteShortlistItemForUserCommand>
{
    public async Task Handle(DeleteShortlistItemForUserCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteShortlistItemForUser(request.Id);
    }
}
