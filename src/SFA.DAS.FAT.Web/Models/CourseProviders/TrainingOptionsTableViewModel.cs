using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class TrainingOptionsTableViewModel
{
    public const string OnlineTraining = "Your learner can complete this training remotely.";

    public const string AtLearnerWorkplaceWithLocationMatchingRegionalOrNational = "The training provider can travel to you to deliver this course.";
    public const string AtLearnerWorkplaceWithLocationNotMatchingRegional = "Training is provided at learner's workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the apprentice's workplace in your location.";

    public const string AtProviderPlaceWithMultipleLocations = "Training provider has multiple locations.";
    public const string ProviderClosestLocationWithLocation = "Training provider’s closest location is";
    public const string ViewAllProviderLocations = "View all training provider locations";
    public const string MilesAway = "miles away:";

    public const string BlockReleaseHint = "Training in blocks of a week or more at the provider's location.";
    public const string BlockReleaseMultipleLocations = "Block release at multiple locations.";
    public const string ViewAllBlockReleaseLocations = "View all block release locations";

    public const string ToClosestLocation = "to closest location:";

    public const string DayReleaseHint = "One day a week at the provider's location.";
    public const string DayReleaseMultipleLocations = "Day release at multiple locations.";
    public const string ViewAllDayReleaseLocations = "View all day release locations";

    public string Location { get; set; }
    public bool HasLocation => !string.IsNullOrWhiteSpace(Location);
    public bool ShowOnlineOption { get; set; }
    public bool ShowLearnerWorkplaceOption { get; set; }
    public string AtLearnerWorkplaceWithNoLocationDisplayMessage { get; set; }
    public bool HasMatchingRegionalLocationOrNational { get; set; }
    public bool ShowProviderOption { get; set; }
    public List<LocationModel> ProviderLocations { get; set; }
    public bool HasMultipleProviderLocations { get; set; }
    public string ClosestProviderLocationDistanceDisplay { get; set; }
    public LocationModel ClosestProviderLocation { get; set; }
    public string ClosestProviderLocationAddress { get; set; }
    public bool ShowBlockReleaseOption { get; set; }
    public List<LocationModel> BlockReleaseLocations { get; set; }
    public bool HasMultipleBlockReleaseLocations { get; set; }
    public string ClosestBlockReleaseLocationDistanceDisplay { get; set; }
    public LocationModel ClosestBlockReleaseLocation { get; set; }
    public bool ShowDayReleaseOption { get; set; }
    public List<LocationModel> DayReleaseLocations { get; set; }
    public bool HasMultipleDayReleaseLocations { get; set; }
    public string ClosestDayReleaseLocationDistanceDisplay { get; set; }
    public LocationModel ClosestDayReleaseLocation { get; set; }
}
