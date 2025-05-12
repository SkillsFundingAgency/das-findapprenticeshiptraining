using System.Collections.Generic;

namespace SFA.DAS.FAT.Application.Locations.Queries.GetLocations
{
    public class GetLocationsQueryResponse
    {
        public IEnumerable<Domain.Locations.LocationItem> LocationItems { get ; set ; }
    }
}