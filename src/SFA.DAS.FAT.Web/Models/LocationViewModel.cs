namespace SFA.DAS.FAT.Web.Models
{
    public class LocationViewModel
    {
        public string Name { get; set; }
        
        public static implicit operator LocationViewModel(Domain.Locations.Locations.LocationItem source)
        {
            return new LocationViewModel
            {
                Name = source.Name
            };
        }
    }
}
