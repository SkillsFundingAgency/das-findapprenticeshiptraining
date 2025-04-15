using MediatR;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Application.Providers.Query.GetProvider;
public class GetProviderQuery : IRequest<GetProviderQueryResponse>
{
    public int Ukprn { get; }

    public GetProviderQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}
