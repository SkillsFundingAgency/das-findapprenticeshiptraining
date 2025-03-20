using System.ComponentModel;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Web.Models.Shared;

public class ProviderRatingViewModel
{
    public required string Stars { get; set; }
    public required string Reviews { get; set; }
    public required ProviderRating ProviderRating { get; set; }
    public required ProviderRatingType ProviderRatingType { get; set; }

    public string TotalMessage
    {
        get
        {
            if (!int.TryParse(Reviews, out int reviews))
            {
                return $"0 {ProviderRatingType.GetDescription()} reviews";
            }

            var totalMessage = $"{reviews} {ProviderRatingType.GetDescription()} review{(reviews != 1 ? "s" : "")}";
            return totalMessage;
        }
    }

    public int StarsValue
    {
        get
        {
            return int.TryParse(Stars, out int stars)
                ? stars
                : 0;
        }
    }

    public string RatingGroup
    {
        get
        {
            return StarsValue > 2 ?
            "good" :
            "poor";
        }
    }
}

public enum ProviderRatingType
{
    [Description("employer")]
    Employer,
    [Description("apprentice")]
    Apprentice
}
