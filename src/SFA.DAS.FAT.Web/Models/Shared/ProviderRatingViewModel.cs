using System.ComponentModel;
using Humanizer;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Web.Models.Shared;

public class ProviderRatingViewModel
{
    private const string GoodRatingGroup = "good";
    private const string PoorRatingGroup = "poor";

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
                reviews = 0;
            }

            var totalMessage = $"{reviews} {ProviderRatingType.GetDescription()} {"review".ToQuantity(reviews, ShowQuantityAs.None)}";
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
            GoodRatingGroup :
            PoorRatingGroup;
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
