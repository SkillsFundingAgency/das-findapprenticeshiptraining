using MediatR;

namespace SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;

public class GetRegisteredProvidersQuery : IRequest<GetRegisteredProvidersResult>
{
    public string Query { get; set; }
}
