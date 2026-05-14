using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesViewModel : PageLinksViewModelBase
{
    public const string BestMatchToCourse = "Best match to course";
    public const string NameOfCourse = "Name of course";
    private const string CoursesSubHeaderText = "Select the course name to view details about it, or select view training providers to see the training providers who run that course.";
    private const string LocationCoursesSubHeaderText = "Select the course name to view details about it, or select view training providers to see the training providers who run that course in the learner's work location.";
    public const string TrainingTypeFindOutMoreText = "Find out more about training types (opens in new tab)";
    public const string TrainingTypeFindOutMoreLink = "https://www.apprenticeships.gov.uk/employers/new-what-is-an-apprenticeship";

    private readonly Dictionary<FilterType, Func<string, string>> _valueFunctions;

    public CoursesViewModel()
    {
        _valueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.Levels, filterValue => GetLevelCodeValue(filterValue) }
        };
    }

    public List<StandardViewModel> Standards { get; set; } = [];

    public List<LevelViewModel> Levels { get; set; } = [];

    public List<RouteViewModel> Routes { get; set; } = [];

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
    public List<LearningType> SelectedTrainingTypes { get; set; } = [];
    public List<int> SelectedLevels { get; set; } = [];

    public int Total { get; set; }

    public int TotalFiltered { get; set; }

    public string SortedDisplayMessage => OrderBy == OrderBy.Score ? BestMatchToCourse : NameOfCourse;

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

    private string PopulateCoursesSubHeader()
    {
        if (Total == 0)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(Location) && Distance != DistanceService.AcrossEnglandFilterValue)
        {
            return LocationCoursesSubHeaderText;
        }

        return CoursesSubHeaderText;
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
                CreateInputFilterSection("keyword-input", KeywordSectionHeading, KeywordSectionSubHeading, nameof(Keyword), Keyword),
                CreateSearchFilterSection("search-location", LocationSectionHeading, LocationSectionSubHeading, nameof(Location), Location),
                CreateDropdownFilterSection("distance-filter", nameof(Distance), DistanceSectionHeading, DistanceSectionSubHeading, GetDistanceFilterValues(Distance).ToList()),
                CreateAccordionFilterSection(
                "multi-select",
                string.Empty,
                [
                    CreateCheckboxListFilterSection(
                        "types-filter",
                        nameof(FilterType.LearningTypes),
                        TrainingTypesSectionHeading,
                        null,
                        LearningTypesFilterHelper.BuildItems(SelectedTrainingTypes),
                        TrainingTypeFindOutMoreText,
                        TrainingTypeFindOutMoreLink
                    ),
                    CreateCheckboxListFilterSection("levels-filter", nameof(Levels), LevelsSectionHeading, null, GenerateLevelFilterItems(), LevelInformationDisplayText, LevelInformationUrl),
                    CreateCheckboxListFilterSection("categories-filter", nameof(FilterType.Categories), CategoriesSectionHeading, null, GenerateRouteFilterItems())
                ]
            )
            ],
            ClearFilterSections = selectedFilterSections
        };
    }

    private List<FilterItemViewModel> GenerateRouteFilterItems()
    {
        return Routes.Select(category => new FilterItemViewModel
        {
            Value = category.Name,
            DisplayText = category.Name,
            IsSelected = SelectedRoutes.Contains(category.Name)
        })
        .ToList();
    }

    private List<FilterItemViewModel> GenerateLevelFilterItems()
    {
        return Levels.Select(level => new FilterItemViewModel
        {
            Value = level.Code.ToString(),
            DisplayText = $"Level {level.Code}",
            DisplayDescription = $"Equal to {level.Name}",
            IsSelected = SelectedLevels.Contains(level.Code)
        })
        .ToList();
    }

    private IReadOnlyList<ClearFilterSectionViewModel> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>();

        AddSelectedFilter(selectedFilters, FilterType.KeyWord, Keyword);
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);
        if (!selectedFilters.ContainsKey(FilterType.Location))
        {
            Distance = DistanceService.TenMiles.ToString();
        }

        if (DistanceService.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
        }

        if (SelectedLevels.Count > 0 && Levels.Count > 0)
        {
            var selectedLevelNames = Levels
                .Where(level => SelectedLevels.Contains(level.Code))
                .Select(level => $"Level {level.Code}")
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Levels, selectedLevelNames);
        }

        if (SelectedRoutes.Count > 0 && Routes.Count > 0)
        {
            var validRoutes = SelectedRoutes
                .Where(route => Routes.Exists(r => r.Name == route))
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.Categories, validRoutes);
        }

        if (SelectedTrainingTypes.Count > 0)
        {
            var validTrainingTypes = SelectedTrainingTypes
                .Where(type => Enum.IsDefined(type))
                .Select(type => type.ToString())
                .ToList();
            AddSelectedFilter(selectedFilters, FilterType.LearningTypes, validTrainingTypes);
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
                case FilterType.LearningTypes:
                    {
                        foreach (var type in SelectedTrainingTypes)
                        {
                            result.Add(ValueTuple.Create(nameof(FilterType.LearningTypes), type.ToString()));
                        }
                    }
                    break;
            }
        }

        return result;
    }
}
