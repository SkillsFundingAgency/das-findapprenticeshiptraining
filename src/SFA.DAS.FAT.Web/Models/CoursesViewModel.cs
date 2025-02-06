using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    private OrderBy _orderBy = OrderBy.None;
    public List<CourseViewModel> Courses { get; set; }
    public string Keyword { get; set; }
    public int? Distance { get; set; }
    public int Total { get; set; }
    public int TotalFiltered { get; set; }
    public List<LevelViewModel> Levels { get; set; }
    public List<RouteViewModel> Routes { get; set; }
    public List<string> SelectedRoutes { get; set; }
    public List<int> SelectedLevels { get; set; }
    public OrderBy OrderBy
    {
        get => _orderBy;
        set
        {
            if (value == OrderBy.None && !string.IsNullOrEmpty(Keyword))
            {
                _orderBy = OrderBy.Relevance;
            }
            else if (value != OrderBy.None && string.IsNullOrEmpty(Keyword))
            {
                _orderBy = OrderBy.None;
            }
            else
            {
                _orderBy = value;
            }
        }
    }

    public bool ShowFilterOptions => ClearLinks.Count > 0;

    public string WorkLocationDisplayMessage => GetWorkLocationDistanceDisplayMessage();

    public string TotalMessage => GetTotalMessage();

    public string OrderByName => $"{GenerateFilterQueryString()}&orderby={nameof(OrderBy.Name).ToLower()}";

    public string OrderByRelevance => $"{GenerateFilterQueryString()}&orderby={nameof(OrderBy.Relevance).ToLower()}";

    private string GetTotalMessage()
    {
        var totalToUse = string.IsNullOrEmpty(Keyword)
                         && (SelectedRoutes == null || !SelectedRoutes.Any())
                         && (SelectedLevels == null || !SelectedLevels.Any())
                                ? Total
                                : TotalFiltered;

        return $"{totalToUse} result" + (totalToUse != 1 ? "s" : "");
    }

    private string GetWorkLocationDistanceDisplayMessage()
    {
        if(!this.Distance.HasValue || this.Distance < 1 || this.Distance > 100)
        {
            return $"{Location} (Across England)";
        }
        else
        {
            return $"{Location} (within {Distance.Value} miles)";
        }
    }

    private Dictionary<CoursesFilterType, Dictionary<string, string>> _clearLinks;

    public Dictionary<CoursesFilterType, Dictionary<string, string>> ClearLinks
    {
        get
        {
            if (_clearLinks == null)
            {
                _clearLinks = GenerateFilterClearLinks();
            }

            return _clearLinks;
        }
    }

    public Dictionary<CoursesFilterType, List<string>> GetSelectedFilters()
    {
        var selectedFilters = new Dictionary<CoursesFilterType, List<string>>();

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            selectedFilters[CoursesFilterType.KeyWord] = new List<string> { Keyword };
        }

        if (!string.IsNullOrWhiteSpace(Location))
        {
            selectedFilters[CoursesFilterType.Location] = new List<string> { Location };
        }

        if (SelectedLevels is not null && SelectedLevels.Count > 0)
        {
            selectedFilters[CoursesFilterType.Levels] = Levels.Where(a => SelectedLevels.Contains(a.Code)).Select(a => a.Name).ToList();
        }

        if (SelectedRoutes is not null && SelectedRoutes.Count > 0)
        {
            selectedFilters[CoursesFilterType.Categories] = SelectedRoutes.Where(a => Routes.Select(t => t.Name).Contains(a)).ToList();
        }

        return selectedFilters;
    }

    public Dictionary<CoursesFilterType, Dictionary<string, string>> GenerateFilterClearLinks()
    {
        var selectedFilters = GetSelectedFilters();

        return GenerateFilterPermutations(selectedFilters);
    }

    public string GenerateFilterQueryString()
    {
        var queryBuilder = new StringBuilder();

        foreach (var param in GetSelectedFilters())
        {
            foreach (var value in param.Value)
            {
                AppendQueryParam(queryBuilder, param.Key, value);
            }
        }

        return queryBuilder.ToString();
    }

    private Dictionary<CoursesFilterType, Dictionary<string, string>> GenerateFilterPermutations(Dictionary<CoursesFilterType, List<string>> queryParams)
    {
        var filterPermutations = new Dictionary<CoursesFilterType, Dictionary<string, string>>();

        foreach (var param in queryParams)
        {
            var clearLinks = new Dictionary<string, string>();

            foreach (var value in param.Value)
            {
                var updatedQuery = BuildQueryWithoutValue(param.Key, value, queryParams);
                clearLinks.Add(value, updatedQuery);
            }

            filterPermutations.Add(param.Key, clearLinks);
        }

        return filterPermutations;
    }

    public string BuildQueryWithoutValue(CoursesFilterType filterType, string value, Dictionary<CoursesFilterType, List<string>> queryParams)
    {
        var queryBuilder = new StringBuilder();

        foreach (var param in queryParams)
        {
            if (param.Key == filterType)
            {
                foreach (var val in param.Value.Where(v => v != value))
                {
                    AppendQueryParam(queryBuilder, param.Key, val);
                }
            }
            else
            {
                foreach (var val in param.Value)
                {
                    AppendQueryParam(queryBuilder, param.Key, val);
                }
            }
        }

        return queryBuilder.ToString();
    }

    private void AppendQueryParam(StringBuilder builder, CoursesFilterType key, string value)
    {
        string stringKey = key.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(value))
        {
            if (builder.Length > 0)
            {
                builder.Append('&');
            }
            else
            {
                builder.Append('?');
            }
            builder.Append($"{stringKey}={GetValue(key, value)}");
        }
    }

    private string GetValue(CoursesFilterType key, string filterValue)
    {
        return key switch
        {
            CoursesFilterType.Levels => GetLevelCodeValue(filterValue),
            _ => filterValue
        };
    }

    private string GetLevelCodeValue(string filterValue)
    {
        var level = Levels.FirstOrDefault(a => a.Name == filterValue);
        return level?.Code.ToString() ?? filterValue;
    }

    public enum CoursesFilterType
    {
        Location,
        Levels,
        Categories,
        KeyWord,
        OrderBy
    }
}
