using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class FeedBackDetails
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

    public List<ProviderAttribute> ProviderAttribute { get; set; }

    public static implicit operator FeedBackDetails(EmployerFeedbackAnnualSummaries source)
    {
        return new FeedBackDetails
        {
            Stars = source.Stars,
            ReviewCount = source.ReviewCount,
            ProviderAttribute = source.ProviderAttribute.Select(x => new ProviderAttribute { Name = x.Name, Strength = x.Strength, Weakness = x.Weakness }).ToList()
        };
    }

    public static implicit operator FeedBackDetails(ApprenticeFeedbackAnnualSummaries source)
    {
        return new FeedBackDetails
        {
            Stars = source.Stars,
            ReviewCount = source.ReviewCount,
            ProviderAttribute = source.ProviderAttribute.Select(x => new ProviderAttribute { Name = x.Name, Strength = x.Strength, Weakness = x.Weakness }).ToList()
        };
    }
}
