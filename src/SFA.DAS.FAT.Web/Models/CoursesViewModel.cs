using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    public List<CourseViewModel> Courses { get; set; } = [];

    public List<LevelViewModel> Levels { get; set; } = [];

    public List<RouteViewModel> Routes { get; set; } = [];
    public string Keyword { get; set; } = string.Empty;

    public int? Distance { get; set; }

    public List<string> SelectedRoutes { get; set; } = [];

    public List<int> SelectedLevels { get; set; } = [];

    public int Total { get; set; }

    public int TotalFiltered { get; set; }

    private OrderBy _orderBy = OrderBy.None;

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

    public string TotalMessage => GetTotalMessage();

    private readonly Dictionary<FilterType, Func<string, string>> ValueFunctions;

    public CoursesViewModel()
    {
        ValueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.Levels, filterValue => GetLevelCodeValue(filterValue) }
        };
    }

    private string GetTotalMessage()
    {
        var totalToUse = string.IsNullOrEmpty(Keyword)
                         && (SelectedRoutes == null || SelectedRoutes.Count < 1)
                         && (SelectedLevels == null || SelectedLevels.Count < 1)
                                ? Total
                                : TotalFiltered;

        return $"{totalToUse} result" + (totalToUse != 1 ? "s" : "");
    }

    private FiltersViewModel? _filters;

    public FiltersViewModel Filters
    {
        get
        {
            if (_filters == null)
            {
                _filters = CreateFilterSections();
            }

            return _filters;
        }
    }

    public FiltersViewModel CreateFilterSections()
    {
        var selectedFilterSections = CreateSelectedFilterSections();

        return new FiltersViewModel()
        {
            Route = RouteNames.Courses,
            FilterSections = new List<FilterSection>
            {
                CreateInputFilterSection("keyword-input", KEYWORD_SECTION_HEADING, KEYWORD_SECTION_SUB_HEADING, nameof(Keyword), Keyword),
                CreateInputFilterSection("search-location", LOCATION_SECTION_HEADING, LOCATION_SECTION_SUB_HEADING, nameof(Location), Location),
                CreateDropdownFilterSection("distance-filter", nameof(Distance), DISTANCE_SECTION_HEADING, DISTANCE_SECTION_SUB_HEADING, GetDistanceFilterValues(Distance)),
                CreateAccordionFilterSection(
                    "multi-select",
                    string.Empty,
                    new List<FilterSection>()
                    {
                        CreateCheckboxListFilterSection("levels-filter", nameof(Levels), LEVELS_SECTION_HEADING, GenerateLevelFilterItems(), LEVEL_INFORMATION_DISPLAY_TEXT, LEVEL_INFORMATION_URL),
                        CreateCheckboxListFilterSection("categories-filter", nameof(FilterType.Categories), CATEGORIES_SECTION_HEADING, GenerateRouteFilterItems())
                    }
                )
            },
            ClearFilterSections = selectedFilterSections
        };
    }

    private List<FilterItem> GenerateRouteFilterItems()
    {
        return Routes?.Select(category => new FilterItem
            {
                Value = category.Name,
                DisplayText = category.Name,
                Selected = SelectedRoutes?.Contains(category.Name) ?? false
            })
        .ToList() ?? new List<FilterItem>();
    }

    private List<FilterItem> GenerateLevelFilterItems()
    {
        return Levels?.Select(level => new FilterItem
            {
                Value = level.Code.ToString(),
                DisplayText = level.Title,
                Selected = SelectedLevels?.Contains(level.Code) ?? false
            })
        .ToList() ?? new List<FilterItem>();
    }

    private IReadOnlyList<ClearFilterSection> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();

        AddSelectedFilter(selectedFilters, FilterType.KeyWord, Keyword);
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);
        if (!selectedFilters.ContainsKey(FilterType.Location))
        {
            Distance = null;
        }

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

        return CreateClearFilterSections(
            selectedFilters,
            ValueFunctions,
            [FilterType.Distance]
        );
    }

    private string GetLevelCodeValue(string filterValue)
    {
        var level = Levels.FirstOrDefault(l => l.Name == filterValue);
        return level?.Code.ToString() ?? string.Empty;
    }
}
