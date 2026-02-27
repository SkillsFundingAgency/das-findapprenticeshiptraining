using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class TrainingOptionsShortCourseViewModel
{
    public required bool IsOnlineAvailable { get; set; }
    public string OnlineDisplayDescription { get; set; }

    public required bool IsEmployerLocationAvailable { get; set; }
    public string EmployerLocationDisplayDescription { get; set; }


    public required bool IsProviderAvailable { get; set; }
    public string ProviderLocationDisplayDescription { get; set; }


    public decimal? NearestEmployerLocation { get; set; }


    public decimal? NearestProviderPlace { get; set; }

    public string Distance { get; set; }
    public string Location { get; set; }

    public string DistanceDetails
    {
        get
        {
            if (string.IsNullOrEmpty(Location))
            {
                return string.Empty;
            }

            var distanceDetails =
                Distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE || string.IsNullOrEmpty(Distance)
                    ? string.Empty
                    : $"within {Distance} miles";

            return distanceDetails;
        }
    }

    public bool ShowDistanceDetails => !string.IsNullOrEmpty(Location);

}
