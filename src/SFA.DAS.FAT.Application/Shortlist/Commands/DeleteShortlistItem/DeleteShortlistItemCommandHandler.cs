using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItem;

public class DeleteShortlistItemCommandHandler(IShortlistService _service) : IRequestHandler<DeleteShortlistItemCommand>
{
    public async Task Handle(DeleteShortlistItemCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteShortlistItem(request.Id);
    }
}
