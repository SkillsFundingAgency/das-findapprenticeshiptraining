using System.Collections.Generic;
using SFA.DAS.FAT.Domain.CourseProviders;

namespace SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;

public class GetCourseProvidersResult
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string LarsCode { get; set; }
    public string StandardName { get; set; }
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }

    public List<ProviderData> Providers { get; set; }
}
