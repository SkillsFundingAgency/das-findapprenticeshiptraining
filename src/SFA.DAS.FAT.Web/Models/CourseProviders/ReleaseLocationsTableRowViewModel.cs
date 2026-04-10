using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class ReleaseLocationsTableRowViewModel
{
    public bool ShowReleaseLocationsOption { get; set; }
    public string TrainingOptionTitle { get; set; }
    public string Hint { get; set; }
    public bool HasLocation { get; set; }
    public bool HasMultipleLocations { get; set; }
    public string MultipleLocationsMessage { get; set; }
    public string ViewAllLocationsText { get; set; }
    public string ClosestLocationDistanceDisplay { get; set; }
    public LocationModel ClosestLocation { get; set; }
    public List<LocationModel> Locations { get; set; }
}
