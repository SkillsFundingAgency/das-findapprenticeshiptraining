using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public class GetCoursesApiRequest : IGetApiRequest
{
    public string Keyword { get; init; }

    public List<int> RouteIds { get; init; }

    public List<int> Levels { get; init; }

    public string TrainingType { get; init; }

    public OrderBy OrderBy { get; init; }

    public int? Distance { get; init; }

    public string Location { get; init; }

    public int Page { get; init; }

    public int PageSize { get; } = Constants.DefaultPageSize;

    public string BaseUrl { get; init; }

    public string GetUrl => BuildUrl();

    private string BuildUrl()
    {
        var queryParams = new List<string>();

        queryParams.Add($"orderby={OrderBy}");

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            queryParams.Add($"keyword={Uri.EscapeDataString(Keyword)}");
        }

        if (!string.IsNullOrWhiteSpace(Location))
        {
            queryParams.Add($"location={Uri.EscapeDataString(Location)}");
        }

        if (Distance.HasValue)
        {
            queryParams.Add($"distance={Distance.Value}");
        }

        if (!string.IsNullOrWhiteSpace(TrainingType))
        {
            queryParams.Add($"apprenticeshipType={TrainingType}");
        }

        if (RouteIds != null && RouteIds.Count > 0)
        {
            foreach (int routeId in RouteIds)
            {
                queryParams.Add($"routeIds={routeId}");
            }
        }

        if (Levels != null && Levels.Count > 0)
        {
            foreach (int level in Levels)
            {
                queryParams.Add($"levels={level}");
            }
        }

        queryParams.Add($"Page={Page}");

        var queryString = string.Join("&", queryParams);

        return $"{BaseUrl}courses{(queryString.Length > 0 ? "?" + queryString : string.Empty)}";
    }
}

