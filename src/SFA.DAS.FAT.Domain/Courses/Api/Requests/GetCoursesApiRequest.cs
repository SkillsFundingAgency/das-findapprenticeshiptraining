using System;
using System.Collections.Generic;
using System.Linq;
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

    public int Page { get; } = 1;

    public int PageSize { get; } = 10;

    public GetCoursesApiRequest(
        string baseUrl, 
        string keyword, 
        string location,
        int? distance,
        List<int> routes, 
        List<int> levels, 
        OrderBy orderBy
    )
    {
        BaseUrl = baseUrl;
        Keyword = keyword;
        Location = location;
        Distance = distance;
        RouteIds = routes;
        Levels = levels;
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

        if (RouteIds != null && RouteIds.Any())
        {
            foreach (int routeId in RouteIds)
            {
                queryParams.Add($"routeIds={routeId}");
            }
        }

        if (Levels != null && Levels.Any())
        {
            foreach(int level in Levels)
            {
                queryParams.Add($"levels={level}");
            }
        }

        var queryString = string.Join("&", queryParams);

        return $"{BaseUrl}courses{(queryString.Any() ? "?" + queryString : string.Empty)}";
    }
}

