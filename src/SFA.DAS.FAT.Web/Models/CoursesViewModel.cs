using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    public List<StandardViewModel> Standards { get; set; } = [];

    public List<LevelViewModel> Levels { get; set; } = [];

    public List<RouteViewModel> Routes { get; set; } = [];

    public List<TypeViewModel> Types { get; set; } = [];

    public PaginationViewModel Pagination { get; set; }

    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// The new keyword hides & overrides the inherited Distance value from PageLinksViewModelBase.
    /// Distance on the inherited PageLinksViewModelBase is used to populate Distance on
    /// the breadcrumbs when viewing course details and cannot have a pre-populated Distance value.
    /// PageLinksViewModelBase Distance property is not required on this model as the Search breadcrumb does not
    /// have the Distance query parameter.
    /// </summary>
    public new string Distance { get; set; } = "All";

    public List<string> SelectedRoutes { get; set; } = [];

    public List<string> SelectedTypes { get; set; } = [];
    public List<int> SelectedLevels { get; set; } = [];

    public int Total { get; set; }

    public int TotalFiltered { get; set; }

    public string SortedDisplayMessage => OrderBy == OrderBy.Score ? BEST_MATCH_TO_COURSE : NAME_OF_COURSE;

    private OrderBy? _orderBy;

    public OrderBy OrderBy
    {
        get
        {
            if (_orderBy == null)
            {
                _orderBy = string.IsNullOrWhiteSpace(Keyword) ? OrderBy.Title : OrderBy.Score;
            }

            return _orderBy.Value;
        }
    }

    private FiltersViewModel _filters;

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

    public string TotalMessage => GetTotalMessage();

    public string CoursesSubHeader => PopulateCoursesSubHeader();

    public const string BEST_MATCH_TO_COURSE = "Best match to course";

    public const string NAME_OF_COURSE = "Name of course";

    private const string _COURSES_SUB_HEADER = "Select the course name to view details about it, or select view training providers to see the training providers who run that course.";

    private const string _LOCATION_COURSES_SUB_HEADER = "Select the course name to view details about it, or select view training providers to see the training providers who run that course in the apprentice's work location.";

    public const string ONE_TRAINING_PROVIDER_MESSAGE = "1 training provider";

    public const string ASK_TRAINING_PROVIDER = "Ask if training providers can run this course";

    public const string APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string APPRENTICESHIP_TYPE_STANDARD_DESCRIPTION = "Apprenticeships that qualify you for a job, levels 2 to 7";

    private readonly Dictionary<FilterType, Func<string, string>> _valueFunctions;

    private readonly IUrlHelper _urlHelper;

    private readonly string _requestApprenticeshipTrainingUrl;

    private readonly string _employerAccountsUrl;

    public CoursesViewModel(FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration, IUrlHelper urlHelper)
    {
        _urlHelper = urlHelper;
        _valueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.Levels, filterValue => GetLevelCodeValue(filterValue) }
        };
        _requestApprenticeshipTrainingUrl = findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl;
        _employerAccountsUrl = findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl;
    }

    public string GetLevelName(int levelCode)
    {
        LevelViewModel level = Levels.Find(a => a.Code == levelCode);

        if (level is null)
        {
            return string.Empty;
        }

        return $"{levelCode} - equal to {level.Name}";
    }

    public string GetProvidersLinkDisplayMessage(StandardViewModel standard)
    {
        if (standard.ProvidersCount < 1)
        {
            return ASK_TRAINING_PROVIDER;
        }

        bool isNationalSearch =
            string.IsNullOrWhiteSpace(Location) ||
            Distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE;

        string providerText =
            standard.ProvidersCount == 1 ?
                ONE_TRAINING_PROVIDER_MESSAGE :
                $"{standard.ProvidersCount} training providers";

        return isNationalSearch
            ? $"View {providerText} for this course"
            : $"View {providerText} within {Distance} miles";
    }

    public string GetProvidersLink(StandardViewModel standard)
    {
        if (standard.ProvidersCount > 0)
        {
            return _urlHelper.RouteUrl(RouteNames.CourseProviders, new { id = standard.LarsCode, Location, distance = Distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE ? "" : Distance })!;
        }

        return GetHelpFindingCourseUrl(standard.LarsCode);
    }

    private string GetHelpFindingCourseUrl(int larsCode)
    {
        string redirectUri = $"{_requestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={larsCode}&requestType={EntryPoint.CourseDetail}";

        var locationQueryParam = !string.IsNullOrEmpty(Location) ? $"&location={Location}" : string.Empty;

        return $"{_employerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    private string PopulateCoursesSubHeader()
    {
        if (Total == 0)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(Location) && Distance != DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
        {
            return _LOCATION_COURSES_SUB_HEADER;
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
                        CreateCheckboxListFilterSection("types-filter", nameof(FilterType.ApprenticeshipTypes), APPRENTICESHIP_TYPES_SECTION_HEADING, null, GenerateApprenticeshipTypesFilterItems()),
                        CreateCheckboxListFilterSection("levels-filter", nameof(Levels), LEVELS_SECTION_HEADING,null, GenerateLevelFilterItems(), LEVEL_INFORMATION_DISPLAY_TEXT, LEVEL_INFORMATION_URL),
                        CreateCheckboxListFilterSection("categories-filter", nameof(FilterType.Categories), CATEGORIES_SECTION_HEADING, null,GenerateRouteFilterItems())
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

    private List<FilterItemViewModel> GenerateApprenticeshipTypesFilterItems()
    {
        return Types?.Select(type => new FilterItemViewModel
        {
            Value = type.Name,
            DisplayText = type.Name,
            DisplayDescription =
                type.Name == ApprenticeshipType.FoundationApprenticeship.GetDescription()
                    ? APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION
                    : APPRENTICESHIP_TYPE_STANDARD_DESCRIPTION,
            Selected = SelectedTypes?.Contains(type.Name) ?? false
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
            Distance = DistanceService.TEN_MILES.ToString();
        }

        if (DistanceService.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
        }

        if (SelectedLevels?.Count > 0 && Levels.Count > 0)
        {
            var selectedLevelNames = Levels
                .Where(level => SelectedLevels.Contains(level.Code))
                .Select(level => $"Level {level.Code}")
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Levels, selectedLevelNames);
        }

        if (SelectedRoutes?.Count > 0 && Routes.Count > 0)
        {
            var validRoutes = SelectedRoutes
                .Where(route => Routes.Exists(r => r.Name == route))
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Categories, validRoutes);
        }

        if (SelectedTypes?.Count > 0 && Types.Count > 0)
        {
            var selectedTypes = Types
                .Where(type => SelectedTypes.Contains(type.Name))
                .Select(type => $"{type.Name}")
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.ApprenticeshipTypes, selectedTypes);
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
        return Levels.Find(l => $"Level {l.Code}" == filterValue)?.Code.ToString() ?? string.Empty;
    }

    public List<ValueTuple<string, string>> ToQueryString()
    {
        List<ValueTuple<string, string>> result = new();

        foreach (ClearFilterSectionViewModel clearFilterSection in Filters.ClearFilterSections)
        {
            switch (clearFilterSection.FilterType)
            {
                case FilterType.KeyWord:
                    {
                        result.Add(ValueTuple.Create(nameof(Keyword), Keyword!));
                    }
                    break;
                case FilterType.Location:
                    {
                        result.Add(ValueTuple.Create(nameof(Location), Location));

                        if (!string.IsNullOrWhiteSpace(Distance))
                        {
                            result.Add(ValueTuple.Create(nameof(Distance), Distance));
                        }
                    }
                    break;
                case FilterType.Levels:
                    {
                        foreach (int level in SelectedLevels)
                        {
                            result.Add(ValueTuple.Create(nameof(FilterType.Levels), level.ToString()));
                        }
                    }
                    break;
                case FilterType.Categories:
                    {
                        foreach (string category in SelectedRoutes)
                        {
                            result.Add(ValueTuple.Create(nameof(FilterType.Categories), category));
                        }
                    }
                    break;
                case FilterType.ApprenticeshipTypes:
                    {
                        foreach (string type in SelectedTypes)
                        {
                            result.Add(ValueTuple.Create(nameof(FilterType.ApprenticeshipTypes), type));
                        }
                    }
                    break;
            }
        }

        return result;
    }

    public Dictionary<string, string> GenerateStandardRouteValues(int larsCode)
    {
        var routeValues = new Dictionary<string, string>
        {
            { "id", larsCode.ToString() },
        };

        if (!string.IsNullOrWhiteSpace(Location))
        {
            routeValues.Add("location", Location);
            routeValues.Add("distance", Distance.ToString());
        }

        return routeValues;
    }
}
