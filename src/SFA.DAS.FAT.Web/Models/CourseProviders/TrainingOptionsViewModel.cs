using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class TrainingOptionsViewModel
{
    public required bool IsEmployerLocationAvailable { get; set; }
    public decimal? NearestEmployerLocation { get; set; }
    public required bool IsBlockReleaseAvailable { get; set; }
    public required bool IsBlockReleaseMultiple { get; set; }
    public decimal? NearestBlockRelease { get; set; }


    public required bool IsDayReleaseAvailable { get; set; }
    public required bool IsDayReleaseMultiple { get; set; }
    public decimal? NearestDayRelease { get; set; }


    public string Distance { get; set; }
    public string Location { get; set; }

    public string DistanceDetails
    {
        get
        {
            var distanceDetails = string.Empty;
            if (!string.IsNullOrEmpty(Location))
            {
                distanceDetails =
                    Distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE || string.IsNullOrEmpty(Distance)
                    ? string.Empty
                    : $"within {Distance} miles";
            }

            return distanceDetails;
        }
    }
    public bool ShowDistanceDetails => !string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(Distance);
}
