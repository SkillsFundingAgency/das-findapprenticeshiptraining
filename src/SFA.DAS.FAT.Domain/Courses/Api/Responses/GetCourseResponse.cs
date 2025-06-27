using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses.Api.Responses;

public sealed class GetCourseResponse
{
    public string StandardUId { get; set; }
    public string IFateReferenceNumber { get; set; }
    public int LarsCode { get; set; }
    public int ProvidersCountWithinDistance { get; set; }
    public int TotalProvidersCount { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public string Version { get; set; }
    public string OverviewOfRole { get; set; }
    public string Route { get; set; }
    public int RouteCode { get; set; }
    public int MaxFunding { get; set; }
    public int TypicalDuration { get; set; }
    public string TypicalJobTitles { get; set; }
    public string StandardPageUrl { get; set; }

    public List<Ksb> Ksbs { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public int IncentivePayment { get; set; }
    public List<RelatedOccupationResponse> RelatedOccupations { get; set; }
}

public class RelatedOccupationResponse
{
    public string Title { get; set; }
    public int Level { get; set; }
}
