using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;

public class GetRegisteredProvidersQueryHandler(IProviderService _providerService) : IRequestHandler<GetRegisteredProvidersQuery, GetRegisteredProvidersResult>
{

    public async Task<GetRegisteredProvidersResult> Handle(GetRegisteredProvidersQuery request, CancellationToken cancellationToken)
    {
        var registeredProviders = await _providerService.GetRegisteredProviders(request.Query);
        return new GetRegisteredProvidersResult { Providers = registeredProviders };
    }
}
