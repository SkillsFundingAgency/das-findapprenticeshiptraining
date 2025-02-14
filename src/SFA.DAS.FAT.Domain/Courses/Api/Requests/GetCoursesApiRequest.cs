using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public class GetCoursesApiRequest : IGetApiRequest
{
    public string Keyword { get; }

    public List<int> RouteIds { get; }

    public List<int> Levels { get; }

    public OrderBy? OrderBy { get; }

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
        var url = $"{BaseUrl}courses?keyword={Keyword}";
        if (OrderBy != Courses.OrderBy.None)
        {
            url += $"&orderby={OrderBy}";
        }

        if(!string.IsNullOrWhiteSpace(Location))
        {
            url += $"&location={Location}";
        }

        if (Distance.HasValue)
        {
            url += $"&distance={Distance.Value}";
        }

        return url;
    }
}

