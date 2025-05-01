using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Locations.Api;

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
        public async Task<Domain.Locations.Locations> GetLocations(string searchTerm)
        {
            var request = new GetLocationsApiRequest(_config.BaseUrl, searchTerm);
            return await _client.Get<Domain.Locations.Locations>(request);
        }

        public async Task<bool> IsLocationValid(string locationName)
        {
            locationName = locationName.Trim();
            if (locationName.Length < 3) return false;

            if (locationName.Contains(','))
            {
                var nameElements = locationName.Split(',');
                var locations = await GetLocations(nameElements[0]);
                return locations != null && locations.LocationItems.Any(x => x.Name == locationName);
            }

            if (locationName.Contains(' '))
            {
                var nameElements = locationName.Split(' ');
                var locations = await GetLocations(nameElements[0]);
                return locations != null && locations.LocationItems.Any(x => x.Name == locationName);
            }

            var result = await GetLocations(locationName);
            return result != null && result.LocationItems.Count > 0;
        }
    }
}
