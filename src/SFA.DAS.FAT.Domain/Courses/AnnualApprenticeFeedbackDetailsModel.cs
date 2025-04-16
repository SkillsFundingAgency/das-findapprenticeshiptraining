using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class AnnualApprenticeFeedbackDetailsModel
{
    public long Ukprn { get; set; }
    public int Stars { get; set; }
    public int ReviewCount { get; set; }
    public IEnumerable<AttributeResultModel> ProviderAttribute { get; set; }
    public string TimePeriod { get; set; }
}
