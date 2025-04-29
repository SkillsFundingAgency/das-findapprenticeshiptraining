using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models.FeedbackSurvey;

public class EmployerFeedBackDetails : FeedbackDetails
{

    public List<EmployerProviderAttribute> ProviderAttributes { get; set; }

    public static implicit operator EmployerFeedBackDetails(EmployerFeedbackAnnualSummaries source)
    {
        return new EmployerFeedBackDetails
        {
            Stars = source.Stars,
            ReviewCount = source.ReviewCount,
            ProviderAttributes = source.ProviderAttribute.Select(x => new EmployerProviderAttribute { Name = x.Name, Strength = x.Strength, Weakness = x.Weakness }).ToList()
        };
    }
}
