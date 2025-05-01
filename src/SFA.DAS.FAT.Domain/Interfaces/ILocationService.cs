using System.Threading.Tasks;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface ILocationService
    {
        Task<Locations.Locations> GetLocations(string searchTerm);
        Task<bool> IsLocationValid(string locationName);
    }
}
