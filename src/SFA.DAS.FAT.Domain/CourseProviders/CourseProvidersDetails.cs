using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.CourseProviders;
public class CourseProvidersDetails
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int LarsCode { get; set; }
    public string StandardName { get; set; }
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }

    public List<ProviderData> Providers { get; set; }
}
