using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models.FeedbackSurvey;

public class ApprenticeFeedBackDetails : FeedbackDetails
{
    public List<ApprenticeProviderAttribute> ProviderAttributes { get; set; }

    public static implicit operator ApprenticeFeedBackDetails(ApprenticeFeedbackAnnualSummaries source)
    {
        return new ApprenticeFeedBackDetails
        {
            Stars = source.Stars,
            ReviewCount = source.ReviewCount,
            ProviderAttributes = source.ProviderAttribute.Select(x => new ApprenticeProviderAttribute { Name = x.Name, Category = x.Category, Agree = x.Agree, Disagree = x.Disagree }).ToList()
        };
    }
}
