using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Locations.Queries.GetLocations
{
    public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsQueryResponse>
    {
        private readonly ILocationService _locationService;

        public GetLocationsQueryHandler (ILocationService locationService)
        {
            _locationService = locationService;
        }
        public async Task<GetLocationsQueryResponse> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            var results = await _locationService.GetLocations(request.SearchTerm);
            
            return new GetLocationsQueryResponse
            {
                LocationItems = results.LocationItems
            }; 
                
        }
    }
}
