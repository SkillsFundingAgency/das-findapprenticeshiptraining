using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Web.Models;

public class StandardViewModel
{
    private const string StandardApprenticeshipDescription = "Standard apprenticeship";

    private const string FoundationApprenticeshipDescription = "Foundation apprenticeship";

    public int Ordering { get; set; }
    public required string StandardUId { get; set; }
    public required string IfateReferenceNumber { get; set; }
    public int LarsCode { get; set; }
    public float? SearchScore { get; set; }
    public int ProvidersCount { get; set; } = 0;
    public int TotalProvidersCount { get; set; } = 0;
    public required string Title { get; set; }
    public int Level { get; set; }
    public required string OverviewOfRole { get; set; }
    public required string Keywords { get; set; }
    public required string Route { get; set; }
    public int RouteCode { get; set; }
    public int MaxFunding { get; set; }
    public int TypicalDuration { get; set; }

    public ApprenticeshipType ApprenticeshipType { get; set; }

    public string ApprenticeTypeDescription { get; set; }

    public static implicit operator StandardViewModel(StandardModel source)
    {
        return new StandardViewModel
        {
            Ordering = source.Ordering,
            StandardUId = source.StandardUId,
            IfateReferenceNumber = source.IfateReferenceNumber,
            LarsCode = source.LarsCode,
            SearchScore = source.SearchScore,
            ProvidersCount = source.ProvidersCount,
            TotalProvidersCount = source.TotalProvidersCount,
            Title = source.Title,
            Level = source.Level,
            OverviewOfRole = source.OverviewOfRole,
            Keywords = source.Keywords,
            Route = source.Route,
            RouteCode = source.RouteCode,
            MaxFunding = source.MaxFunding,
            TypicalDuration = source.TypicalDuration,
            ApprenticeshipType = source.ApprenticeshipType,
            ApprenticeTypeDescription = source.ApprenticeshipType == ApprenticeshipType.FoundationApprenticeship
                    ? FoundationApprenticeshipDescription
                    : StandardApprenticeshipDescription
        };
    }
}
