using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Web.Models.FeedbackSurvey;

public class FeedbackDetails
{
    public int Stars { get; set; }
    public int ReviewCount { get; set; }

    public string ReviewMessage => ReviewCount == 1 ? $"1 review" : $"{ReviewCount} reviews";

    public string Rating
    {
        get =>
            Stars switch
            {
                4 => ProviderRating.Excellent.GetDescription(),
                3 => ProviderRating.Good.GetDescription(),
                2 => ProviderRating.Poor.GetDescription(),
                1 => ProviderRating.VeryPoor.GetDescription(),
                _ => ProviderRating.NotYetReviewed.GetDescription()
            };
    }
}
