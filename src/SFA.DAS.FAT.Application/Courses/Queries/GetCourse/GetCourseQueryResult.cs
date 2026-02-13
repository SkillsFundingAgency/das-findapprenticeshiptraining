using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public class GetCourseQueryResult
{
    public string StandardUId { get; set; }
    public string IFateReferenceNumber { get; set; }
    public string LarsCode { get; set; }
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

    public int IncentivePayment { get; set; }
    public List<RelatedOccupation> RelatedOccupations { get; set; }
    public List<Ksb> Ksbs { get; set; } = [];

    public ApprenticeshipType TrainingType { get; set; }

    public List<Level> Levels { get; set; } = [];

    public static implicit operator GetCourseQueryResult(GetCourseResponse source)
    {
        return new()
        {
            StandardUId = source.StandardUId,
            IFateReferenceNumber = source.IFateReferenceNumber,
            LarsCode = source.LarsCode,
            ProvidersCountWithinDistance = source.ProvidersCountWithinDistance,
            TotalProvidersCount = source.TotalProvidersCount,
            Title = source.Title,
            Level = source.Level,
            Version = source.Version,
            OverviewOfRole = source.OverviewOfRole,
            Route = source.Route,
            RouteCode = source.RouteCode,
            MaxFunding = source.MaxFunding,
            TypicalDuration = source.TypicalDuration,
            TypicalJobTitles = source.TypicalJobTitles,
            StandardPageUrl = source.StandardPageUrl,
            IncentivePayment = source.IncentivePayment,
            Ksbs = source.Ksbs,
            RelatedOccupations = source.RelatedOccupations == null
                                    ? new List<RelatedOccupation>() :
                                    source.RelatedOccupations.Select(c => (RelatedOccupation)c).ToList(),
            TrainingType = source.ApprenticeshipType,
        };
    }
}
