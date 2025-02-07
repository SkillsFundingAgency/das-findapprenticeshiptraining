using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Filters;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using static SFA.DAS.FAT.Domain.Courses.Filters.CourseFilters;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    public List<CourseViewModel> Courses { get; set; }

    public List<LevelViewModel> Levels { get; set; }

    public List<RouteViewModel> Routes { get; set; }

    public List<string> SelectedRoutes { get; set; }

    public List<int> SelectedLevels { get; set; }

    private OrderBy _orderBy = OrderBy.None;

    public string Keyword { get; set; }

    public int? Distance { get; set; }

    public int Total { get; set; }

    public int TotalFiltered { get; set; }
    
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

    public bool ShowFilterOptions => SelectedFilterSections.Count > 0;

    public string WorkLocationDisplayMessage => GetWorkLocationDistanceDisplayMessage();

    public string TotalMessage => GetTotalMessage();

    private string GetTotalMessage()
    {
        var totalToUse = string.IsNullOrEmpty(Keyword)
                         && (SelectedRoutes == null || SelectedRoutes.Count < 1)
                         && (SelectedLevels == null || SelectedLevels.Count < 1)
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

    private List<ClearFilterSection> _selectedFilterSections;

    public List<ClearFilterSection> SelectedFilterSections
    {
        get
        {
            if (_selectedFilterSections == null)
            {
                _selectedFilterSections = GenerateSelectedFilterSections();
            }
            return _selectedFilterSections;
        }
    }

    private List<ClearFilterSection> GenerateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();

        AddSelectedFilter(selectedFilters, FilterType.KeyWord, Keyword);
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);

        if (Distance.HasValue && ValidDistances.IsValidDistance(Distance.Value))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance.Value.ToString());
        }

        if (SelectedLevels?.Count > 0 && Levels.Count > 0)
        {
            var selectedLevelNames = Levels
                .Where(level => SelectedLevels.Contains(level.Code))
                .Select(level => level.Name)
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Levels, selectedLevelNames);
        }

        if (SelectedRoutes?.Count > 0 && Routes.Count > 0)
        {
            var validRoutes = SelectedRoutes
                .Where(route => Routes.Any(r => r.Name == route))
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Categories, validRoutes);
        }

        if (selectedFilters.Count == 0)
        {
            return [];
        }

        return selectedFilters
            .Where(filter => filter.Key != FilterType.Distance)
            .Select(filter => new ClearFilterSection
            {
                Title = ClearFilterSectionHeadings[filter.Key],
                Items = filter.Value
                    .Select(value => new ClearFilterItem
                    {
                        DisplayText = GetDisplayValue(filter.Key, value),
                        ClearLink = BuildQueryWithoutValue(filter.Key, value, selectedFilters)
                    })
                    .ToList()
            })
            .ToList();
    }

    private static void AddSelectedFilter(Dictionary<FilterType, List<string>> filters, FilterType type, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            filters[type] = new List<string> { value };
        }
    }

    private static void AddSelectedFilter(Dictionary<FilterType, List<string>> filters, FilterType type, List<string> values)
    {
        if (values?.Count > 0)
        {
            filters[type] = values;
        }
    }

    private string BuildQueryWithoutValue(FilterType filterType, string value, Dictionary<FilterType, List<string>> queryParams)
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

    private void AppendQueryParam(StringBuilder builder, FilterType key, string value)
    {
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

            builder.Append($"{key.ToString().ToLower()}={GetQueryValue(key, value)}");
        }
    }

    private string GetQueryValue(FilterType key, string filterValue)
    {
        return key switch
        {
            FilterType.Levels => GetLevelCodeValue(filterValue),
            _ => filterValue
        };
    }

    private string GetDisplayValue(FilterType key, string displayValue)
    {
        return key switch
        {
            FilterType.Location => GetWorkLocationDistanceDisplayMessage(),
            _ => displayValue
        };
    }

    private string GetLevelCodeValue(string filterValue)
    {
        var level = Levels.FirstOrDefault(a => a.Name == filterValue);
        return level?.Code.ToString() ?? filterValue;
    }
}
