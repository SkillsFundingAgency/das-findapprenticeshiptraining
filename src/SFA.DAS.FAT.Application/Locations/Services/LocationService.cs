using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Locations.Services
{
    public class LocationService : ILocationService
    {
        private readonly IApiClient _client;
        private readonly FindApprenticeshipTrainingApi _config;

        public LocationService(IApiClient client, IOptions<FindApprenticeshipTrainingApi> config)
        {
            _client = client;
            _config = config.Value;
        }
        public async Task<Domain.Locations> GetLocations(string searchTerm)
        {
            if (searchTerm.Trim().Length < 3) return new Domain.Locations { LocationItems = new List<Domain.Locations.LocationItem>() };
            var request = new GetLocationsApiRequest(_config.BaseUrl, searchTerm);
            return await _client.Get<Domain.Locations>(request);
        }

        public async Task<bool> IsLocationValid(string locationName)
        {
            locationName = locationName.Trim();

            if (locationName.Contains(','))
            {
                var firstItem = locationName.Split(',').First().Trim();
                var locations = await GetLocations(firstItem);
                return locations.LocationItems.Any(x => x.Name == locationName);
            }

            var result = await GetLocations(locationName);

            if (result.LocationItems.Count > 0) return true;

            if (locationName.Contains(' '))
            {
                var firstItem = locationName.Split(' ').First().Trim();
                var locations = await GetLocations(firstItem);
                return locations.LocationItems.Any(x => x.Name == locationName);
            }

            return false;
        }
    }
}
