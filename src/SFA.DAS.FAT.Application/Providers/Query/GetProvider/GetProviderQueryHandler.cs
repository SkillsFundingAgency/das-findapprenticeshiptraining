using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers.Api.Requests;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Application.Providers.Query.GetProvider;

public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, GetProviderQueryResponse>
{
    private readonly IApiClient _client;
    private readonly FindApprenticeshipTrainingApi _config;

    public GetProviderQueryHandler(IApiClient client, IOptions<FindApprenticeshipTrainingApi> config)
    {
        _client = client;
        _config = config.Value;
    }

    public async Task<GetProviderQueryResponse> Handle(GetProviderQuery query, CancellationToken cancellationToken)
    {
        var request = new GetProviderApiRequest(_config.BaseUrl, query.Ukprn);

        var response = await _client.Get<GetProviderQueryResponse>(request);

        return response;
    }
}
