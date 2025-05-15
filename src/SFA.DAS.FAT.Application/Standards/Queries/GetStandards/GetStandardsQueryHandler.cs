using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Standards;

namespace SFA.DAS.FAT.Application.Standards.Queries.GetStandards;

public class GetStandardsQueryHandler(IApiClient _client, IOptions<FindApprenticeshipTrainingApi> _config) : IRequestHandler<GetStandardsQuery, GetStandardsQueryResult>
{
    public async Task<GetStandardsQueryResult> Handle(GetStandardsQuery request, CancellationToken cancellationToken)
    {
        var apiRequest = new GetStandardsApiRequest(_config.Value.BaseUrl);

        var response = await _client.Get<GetStandardsQueryResult>(apiRequest);

        return response;
    }
}


