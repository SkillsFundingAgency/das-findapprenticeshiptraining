using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class CourseProvidersViewModel : PageLinksViewModelBase
{
    public int ShortlistCount { get; set; }
    public ProviderOrderBy OrderBy { get; set; }
    public string CourseTitleAndLevel { get; set; } = string.Empty;
    public CourseType CourseType { get; set; }
    public LearningType ApprenticeshipType { get; set; }
    public bool IsActiveAvailable { get; set; }
    public IEnumerable<string> SelectedDeliveryModes { get; set; } = [];
    public IEnumerable<string> SelectedEmployerApprovalRatings { get; set; } = [];
    public IEnumerable<string> SelectedApprenticeApprovalRatings { get; set; } = [];
    public IEnumerable<string> SelectedQarRatings { get; set; } = [];
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }
    public string QarPeriodStartYear => $"20{QarPeriod.AsSpan(0, 2)}";
    public string QarPeriodEndYear => $"20{QarPeriod.AsSpan(2, 2)}";
    public string ReviewPeriodStartYear => $"20{ReviewPeriod.AsSpan(0, 2)}";
    public string ReviewPeriodEndYear => $"20{ReviewPeriod.AsSpan(2, 2)}";
    public string ProviderReviewsHeading => $"Provider reviews in {ReviewPeriodStartYear} to {ReviewPeriodEndYear}";
    public string CourseAchievementRateHeading => $"Course achievement rate in {QarPeriodStartYear} to {QarPeriodEndYear}";
    public List<CoursesProviderViewModel> Providers { get; set; } = [];
    public List<ProviderOrderByOptionViewModel> ProviderOrderOptions { get; set; } = [];
    public PaginationViewModel Pagination { get; set; }
    public int TotalCount { get; set; }
    private readonly string _requestApprenticeshipTrainingUrl;
    private readonly string _employerAccountsUrl;
    private readonly Dictionary<FilterType, Func<string, string>> _valueFunctions;
    private FiltersViewModel _filters;

    public string TotalMessage => GetTotalMessage();

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
    public CourseProvidersViewModel(FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration)
    {
        _valueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.DeliveryModes, GetDeliveryModeValue },
            { FilterType.QarRatings, GetQarRatingsValue },
            { FilterType.EmployerProviderRatings, GetProviderRatingsValue },
            { FilterType.ApprenticeProviderRatings, GetProviderRatingsValue }
        };
        _requestApprenticeshipTrainingUrl = findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl;
        _employerAccountsUrl = findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl;
    }

    public FiltersViewModel CreateFilterSections()
    {
        var selectedFilterSections = CreateSelectedFilterSections();

        return new FiltersViewModel()
        {
            Route = RouteNames.CourseProviders,
            FilterSections =
            [
                CreateSearchFilterSection("search-location", LocationSectionHeading, LocationSectionSubHeading, nameof(Location), Location),
                CreateDropdownFilterSection("distance-filter", nameof(Distance), DistanceSectionHeading, DistanceSectionSubHeading, GetDistanceFilterValues(Distance).ToList()),
                CreateCheckboxListFilterSection("modes-filter", nameof(FilterType.DeliveryModes), DeliveryModesSectionHeading, DeliveryModesSectionSubHeading, GenerateDeliveryModesFilterItems()),
                CreateAccordionGroupFilterSection(
                    "ratings-select",
                    nameof(FilterType.Reviews),
                    [
                        CreateCheckboxListFilterSection("employer-ratings-filter", nameof(FilterType.EmployerProviderRatings), EmployerReviewsSectionHeading,null, GenerateEmployerReviewsFilterItems()),
                        CreateCheckboxListFilterSection("apprentice-ratings-filter", nameof(FilterType.ApprenticeProviderRatings), ApprenticeReviewsSectionHeading,null, GenerateApprenticeReviewsFilterItems())
                    ],
                    ReviewSectionHeading,
                    $"From {ReviewPeriodStartYear} to {ReviewPeriodEndYear}"
                ),
                CreateAccordionFilterSection(
                    "qar-select",
                    string.Empty,
                    [
                        CreateCheckboxListFilterSection(
                            "qar-filter",
                            nameof(FilterType.QarRatings),
                            QarSectionHeading,
                            $"From {QarPeriodStartYear} to {QarPeriodEndYear}",
                            GenerateQarFilterItems()
                        )
                    ])
                ],
            ClearFilterSections = selectedFilterSections
        };
    }

    public string GetHelpFindingCourseUrl(string larsCode)
    {
        var redirectUri = $"{_requestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={larsCode}&requestType={EntryPoint.CourseDetail}";

        var locationQueryParam = !string.IsNullOrEmpty(Location) ? $"&location={Location}" : string.Empty;

        return $"{_employerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    public List<ValueTuple<string, string>> ToQueryString()
    {
        List<ValueTuple<string, string>> result = new();

        var defaultOrderBy = CourseType == CourseType.ShortCourse
            ? ProviderOrderBy.Distance
            : ProviderOrderBy.AchievementRate;

        if (OrderBy != defaultOrderBy)
        {
            result.Add(ValueTuple.Create(nameof(OrderBy), OrderBy.ToString()));
        }

        foreach (ClearFilterSectionViewModel clearFilterSection in Filters.ClearFilterSections)
        {
            switch (clearFilterSection.FilterType)
            {

                case FilterType.Location:
                    {
                        result.Add(ValueTuple.Create(nameof(Location), Location));

                        result.Add(!string.IsNullOrWhiteSpace(Distance)
                            ? ValueTuple.Create(nameof(Distance), Distance)
                            : ValueTuple.Create(nameof(Distance), DistanceService.AcrossEnglandFilterValue));
                    }
                    break;


                case FilterType.DeliveryModes:
                    {
                        result.AddRange(SelectedDeliveryModes.Select(deliveryMode => ValueTuple.Create(nameof(FilterType.DeliveryModes), deliveryMode)));
                    }
                    break;
                case FilterType.EmployerProviderRatings:
                    {
                        result.AddRange(SelectedEmployerApprovalRatings.Select(ratings => ValueTuple.Create(nameof(FilterType.EmployerProviderRatings), ratings)));
                    }
                    break;
                case FilterType.ApprenticeProviderRatings:
                    {
                        result.AddRange(SelectedApprenticeApprovalRatings.Select(ratings => ValueTuple.Create(nameof(FilterType.ApprenticeProviderRatings), ratings)));
                    }
                    break;
                case FilterType.QarRatings:
                    {
                        result.AddRange(SelectedQarRatings.Select(ratings => ValueTuple.Create(nameof(FilterType.QarRatings), ratings)));
                    }
                    break;
            }
        }

        return result;
    }


    private string GetTotalMessage()
    {
        var totalToUse = TotalCount <= 0 ? "No" : TotalCount.ToString();
        var totalMessage = $"{totalToUse} result{(TotalCount == 1 ? string.Empty : "s")}";

        if (!string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(Distance) &&
            Distance != DistanceService.AcrossEnglandFilterValue)
        {
            totalMessage = $"{totalMessage} within {Distance} miles";
        }

        return totalMessage;
    }

    private static List<DeliveryModeOptionViewModel> BuildDeliveryModeOptionViewModel()
    {
        return Enum.GetValues<ProviderDeliveryMode>()
            .Select(deliveryModeChoice => new DeliveryModeOptionViewModel
            {
                DeliveryItemChoice = deliveryModeChoice,
                Description = deliveryModeChoice.GetDescription(),
                Selected = false
            })
            .ToList();
    }

    private static List<QarOptionViewModel> BuildQarRatingsViewModel()
    {
        return Enum.GetValues<QarRating>()
            .Select(rating => new QarOptionViewModel
            {
                QarRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            })
            .ToList();
    }

    private static List<EmployerProviderRatingOptionViewModel> BuildEmployerProviderRatingsViewModel()
    {
        return Enum.GetValues<ProviderRating>()
            .Select(rating => new EmployerProviderRatingOptionViewModel
            {
                ProviderRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            })
            .ToList();
    }

    private static List<ApprenticeProviderRatingOptionViewModel> BuildApprenticeProviderRatingsViewModel()
    {
        return Enum.GetValues<ProviderRating>()
            .Select(rating => new ApprenticeProviderRatingOptionViewModel
            {
                ProviderRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            })
            .ToList();
    }

    private List<FilterItemViewModel> GenerateDeliveryModesFilterItems()
    {
        var deliveryModes = BuildDeliveryModeOptionViewModel();

        return deliveryModes.Select(deliveryMode => new FilterItemViewModel
        {
            Value = deliveryMode.DeliveryItemChoice.ToString(),
            DisplayText = deliveryMode.DeliveryItemChoice.GetDescription(),
            DisplayDescription = deliveryMode.DeliveryItemChoice switch
            {
                ProviderDeliveryMode.Online => FilterService.DeliveryModesSectionOnlineDisplayDescription,
                ProviderDeliveryMode.Workplace => FilterService.DeliveryModesSectionWorkplaceDisplayDescription,
                ProviderDeliveryMode.Provider => FilterService.DeliveryModesSectionProviderDisplayDescription,
                _ => string.Empty
            },
            IsSelected = SelectedDeliveryModes?.Contains(deliveryMode.DeliveryItemChoice.ToString()) ?? false
        })
            .ToList();
    }

    private List<FilterItemViewModel> GenerateQarFilterItems()
    {
        var qarRatings = BuildQarRatingsViewModel();

        return qarRatings.Select(qarRating => new FilterItemViewModel
        {
            Value = qarRating.QarRatingType.ToString(),
            DisplayText = qarRating.QarRatingType.GetDescription(),
            IsSelected = SelectedQarRatings?.Contains(qarRating.QarRatingType.ToString()) ?? false
        })
            .ToList();
    }

    private List<FilterItemViewModel> GenerateEmployerReviewsFilterItems()
    {
        var ratings = BuildEmployerProviderRatingsViewModel();

        return ratings.Select(rating => new FilterItemViewModel
        {
            Value = rating.ProviderRatingType.ToString(),
            DisplayText =
                    rating.ProviderRatingType == ProviderRating.NotYetReviewed
                    ? "No employer reviews"
                    : rating.ProviderRatingType.GetDescription(),
            IsSelected = SelectedEmployerApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
        })
            .ToList();
    }

    private List<FilterItemViewModel> GenerateApprenticeReviewsFilterItems()
    {
        var ratings = BuildApprenticeProviderRatingsViewModel();

        return ratings.Select(rating => new FilterItemViewModel
        {
            Value = rating.ProviderRatingType.ToString(),
            DisplayText =
                    rating.ProviderRatingType == ProviderRating.NotYetReviewed
                        ? "No apprentice reviews"
                        : rating.ProviderRatingType.GetDescription(),
            IsSelected = SelectedApprenticeApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
        })
            .ToList();
    }

    private IReadOnlyList<ClearFilterSectionViewModel> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, IEnumerable<string>>();

        AddLocationAndDistanceFilters(selectedFilters);
        AddDeliveryModesFilter(selectedFilters);
        AddRatingFilters(selectedFilters);

        var defaultOrderBy = (CourseType == CourseType.ShortCourse)
            ? ProviderOrderBy.Distance
            : ProviderOrderBy.AchievementRate;

        if (OrderBy != defaultOrderBy)
        {
            AddSelectedFilter(selectedFilters, FilterType.OrderBy, OrderBy.ToString());
        }

        if (selectedFilters.Count == 0)
        {
            return [];
        }

        return CreateClearFilterSections(
            selectedFilters,
            _valueFunctions,
            new[] { FilterType.Distance, FilterType.OrderBy });
    }

    private void AddLocationAndDistanceFilters(Dictionary<FilterType, IEnumerable<string>> selectedFilters)
    {
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);

        if (!selectedFilters.ContainsKey(FilterType.Location) && string.IsNullOrEmpty(Distance))
        {
            Distance = DistanceService.TenMiles.ToString();
        }

        if (DistanceService.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
        }
    }

    private void AddDeliveryModesFilter(Dictionary<FilterType, IEnumerable<string>> selectedFilters)
    {
        if (SelectedDeliveryModes.Any())
        {
            var deliveryModes = GenerateDeliveryModesFilterItems();
            var selectedDeliveryNames = deliveryModes
                .Where(dm => SelectedDeliveryModes.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            if (CourseType == CourseType.ShortCourse)
            {
                selectedFilters[FilterType.DeliveryModes] = selectedDeliveryNames;
            }
            else
            {
                AddSelectedFilter(selectedFilters, FilterType.DeliveryModes, selectedDeliveryNames);
            }
        }
    }

    private void AddRatingFilters(Dictionary<FilterType, IEnumerable<string>> selectedFilters)
    {
        AddRatingFilter(selectedFilters, SelectedEmployerApprovalRatings, GenerateEmployerReviewsFilterItems, FilterType.EmployerProviderRatings);
        AddRatingFilter(selectedFilters, SelectedApprenticeApprovalRatings, GenerateApprenticeReviewsFilterItems, FilterType.ApprenticeProviderRatings);
        AddRatingFilter(selectedFilters, SelectedQarRatings, GenerateQarFilterItems, FilterType.QarRatings);
    }

    private static void AddRatingFilter(Dictionary<FilterType, IEnumerable<string>> selectedFilters, IEnumerable<string> selectedRatings, Func<List<FilterItemViewModel>> generateFilterItems, FilterType filterType)
    {
        if (selectedRatings?.Any() ?? false)
        {
            var filterItems = generateFilterItems();
            var selectedItems = filterItems
                .Where(item => selectedRatings.Contains(item.Value.ToString()))
                .Select(item => item.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, filterType, selectedItems);
        }
    }

    private static string GetDeliveryModeValue(string filterValue)
    {
        return BuildDeliveryModeOptionViewModel()
            .Where(dm => dm.Description == filterValue)
            .Select(dm => dm.DeliveryItemChoice.ToString())
            .FirstOrDefault(filterValue);
    }

    private static string GetQarRatingsValue(string filterValue)
    {
        return BuildQarRatingsViewModel()
            .Where(qar => qar.Description == filterValue)
            .Select(qar => qar.QarRatingType.ToString())
            .FirstOrDefault(filterValue);
    }

    private static string GetProviderRatingsValue(string filterValue)
    {
        return BuildEmployerProviderRatingsViewModel()
            .Where(rating => rating.Description == filterValue)
            .Select(rating => rating.ProviderRatingType.ToString())
            .FirstOrDefault(filterValue);
    }
}
