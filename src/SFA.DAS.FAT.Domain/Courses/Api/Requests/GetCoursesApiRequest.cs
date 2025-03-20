using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public class GetCoursesApiRequest : IGetApiRequest
{
    public string Keyword { get; }

    public List<int> RouteIds { get; }

    public List<int> Levels { get; }

    public OrderBy OrderBy { get; }

    public int? Distance { get; }

    public string Location { get; }

    public int Page { get; }

    public int PageSize { get; } = Constants.DefaultPageSize;

    public GetCoursesApiRequest(
        string baseUrl,
        string keyword,
        string location,
        int? distance,
        List<int> routes,
        List<int> levels,
        int page,
        OrderBy orderBy
    )
    {
        BaseUrl = baseUrl;
        Keyword = keyword;
        Location = location;
        Distance = distance;
        RouteIds = routes;
        Levels = levels;
        Page = page;
        OrderBy = orderBy;
    }

    public string BaseUrl { get; }

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

