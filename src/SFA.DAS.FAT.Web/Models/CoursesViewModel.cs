using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    public List<StandardViewModel> Standards { get; set; } = [];

    public List<LevelViewModel> Levels { get; set; } = [];

    public List<RouteViewModel> Routes { get; set; } = [];

    public string? Keyword { get; set; } = string.Empty;

    public string? Distance { get; set; } = "All";

    public List<string> SelectedRoutes { get; set; } = [];

    public List<int> SelectedLevels { get; set; } = [];

    public int Total { get; set; }

    public int TotalFiltered { get; set; }

    public string SortedDisplayMessage => GetSortedDisplayMessage();

    private OrderBy? _orderBy;

    public OrderBy OrderBy
    {
        get
        {
            if (_orderBy == null)
            {
                _orderBy = SetOrderBy();
            }

            return _orderBy.Value;
        }
    }

    public string TotalMessage => GetTotalMessage();

    public string CoursesSubHeader => PopulateCoursesSubHeader();

    private readonly Dictionary<FilterType, Func<string, string>> _valueFunctions;

    private readonly FindApprenticeshipTrainingWeb _findApprenticeshipTrainingWebConfiguration;

    private readonly IUrlHelper _urlHelper;

    private readonly string _requestApprenticeshipTrainingUrl;

    public CoursesViewModel(FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration, IUrlHelper urlHelper)
    {
        _findApprenticeshipTrainingWebConfiguration = findApprenticeshipTrainingWebConfiguration;
        _urlHelper = urlHelper;
        _valueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.Levels, filterValue => GetLevelCodeValue(filterValue) }
        };
        _requestApprenticeshipTrainingUrl = _findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl;
    }

    private OrderBy SetOrderBy()
    {
        return string.IsNullOrWhiteSpace(Keyword) ? OrderBy.Title : OrderBy.Score;
    }

    private const string _BEST_MATCH_TO_COURSE = "Best match to course";

    private const string _NAME_OF_COURSE = "Name of course";

    private string GetSortedDisplayMessage()
    {
        return OrderBy == OrderBy.Score ?
            _BEST_MATCH_TO_COURSE :
            _NAME_OF_COURSE;
    }

    public string GetLevelName(int levelCode)
    {
        LevelViewModel? level = Levels.FirstOrDefault(a => a.Code == levelCode);

        if (level is null)
        {
            return string.Empty;
        }

        return $"{levelCode} (equal to {level.Name})";
    }

    private const string _VIEW_ONE_TRAINING_PROVIDER_FOR_THIS_COURSE = "View 1 training provider for this course.";

    private const string _ASK_TRAINING_PROVIDER = "Ask if training providers can run this course.";

    public static string GetProvidersLinkDisplayMessage(StandardViewModel standard)
    {
        if (standard.ProvidersCount == 1)
        {
            return _VIEW_ONE_TRAINING_PROVIDER_FOR_THIS_COURSE;
        }
        else if (standard.ProvidersCount > 1)
        {
            return $"View {standard.ProvidersCount} training providers for this course.";
        }
        else
        {
            return _ASK_TRAINING_PROVIDER;
        }
    }

    public string GetProvidersLink(StandardViewModel standard)
    {
        if (standard.ProvidersCount > 0)
        {
            return _urlHelper.RouteUrl(RouteNames.CourseProviders, new { id = standard.LarsCode })!;
        }
        
        return _requestApprenticeshipTrainingUrl;
    }

    private const string _COURSES_SUB_HEADER = "Select the course name to view details about it, or select view training providers to see the training providers who run that course.";

    private string PopulateCoursesSubHeader()
    {
        if(Total == 0)
        {
            return string.Empty;
        }

        return _COURSES_SUB_HEADER;
    }

    private string GetTotalMessage()
    {
        var totalToUse = string.IsNullOrEmpty(Keyword)
                         && (SelectedRoutes == null || SelectedRoutes.Count < 1)
                         && (SelectedLevels == null || SelectedLevels.Count < 1)
                                ? Total
                                : TotalFiltered;

        string resultDisplayMessage = $"{totalToUse} result";
        if (totalToUse != 1)
        {
            resultDisplayMessage += "s";
        }

        return resultDisplayMessage;
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
            FilterSections = 
            [
                CreateInputFilterSection("keyword-input", KEYWORD_SECTION_HEADING, KEYWORD_SECTION_SUB_HEADING, nameof(Keyword), Keyword),
                CreateSearchFilterSection("search-location", LOCATION_SECTION_HEADING, LOCATION_SECTION_SUB_HEADING, nameof(Location), Location),
                CreateDropdownFilterSection("distance-filter", nameof(Distance), DISTANCE_SECTION_HEADING, DISTANCE_SECTION_SUB_HEADING, GetDistanceFilterValues(Distance)),
                CreateAccordionFilterSection(
                    "multi-select",
                    string.Empty,
                    [
                        CreateCheckboxListFilterSection("levels-filter", nameof(Levels), LEVELS_SECTION_HEADING, GenerateLevelFilterItems(), LEVEL_INFORMATION_DISPLAY_TEXT, LEVEL_INFORMATION_URL),
                        CreateCheckboxListFilterSection("categories-filter", nameof(FilterType.Categories), CATEGORIES_SECTION_HEADING, GenerateRouteFilterItems())
                    ]
                )
            ],
            ClearFilterSections = selectedFilterSections
        };
    }

    private List<FilterItemViewModel> GenerateRouteFilterItems()
    {
        return Routes?.Select(category => new FilterItemViewModel
            {
                Value = category.Name,
                DisplayText = category.Name,
                Selected = SelectedRoutes?.Contains(category.Name) ?? false
            })
        .ToList() ?? [];
    }

    private List<FilterItemViewModel> GenerateLevelFilterItems()
    {
        return Levels?.Select(level => new FilterItemViewModel
            {
                Value = level.Code.ToString(),
                DisplayText = $"Level {level.Code}",
                DisplayDescription = $"Equal to {level.Name}",
                Selected = SelectedLevels?.Contains(level.Code) ?? false
            })
        .ToList() ?? [];
    }

    private IReadOnlyList<ClearFilterSectionViewModel> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();

        AddSelectedFilter(selectedFilters, FilterType.KeyWord, Keyword);
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);
        if (!selectedFilters.ContainsKey(FilterType.Location))
        {
            Distance = ValidDistances.ACROSS_ENGLAND_FILTER_VALUE;
        }

        if (ValidDistances.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
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
            _valueFunctions,
            [FilterType.Distance]
        );
    }

    private string GetLevelCodeValue(string filterValue)
    {
        return Levels.FirstOrDefault(l => l.Name == filterValue)?.Code.ToString() ?? string.Empty;
    }
}
