using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;

namespace SFA.DAS.FAT.Domain.Courses.Api.Responses;

public sealed class GetCoursesResponse
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Constants.DefaultPageSize;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public List<StandardModel> Standards { get; set; } = new List<StandardModel>();
}

public sealed class StandardModel
{
    public int Ordering { get; set; }
    public string StandardUId { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string LarsCode { get; set; }
    public float? SearchScore { get; set; }
    public int ProvidersCount { get; set; } = 0;
    public int TotalProvidersCount { get; set; } = 0;
    public string Title { get; set; }
    public int Level { get; set; }
    public string OverviewOfRole { get; set; }
    public string Keywords { get; set; }
    public string Route { get; set; }
    public int RouteCode { get; set; }
    public int MaxFunding { get; set; }
    public int TypicalDuration { get; set; }

    public ApprenticeshipType ApprenticeshipType { get; set; }
}
