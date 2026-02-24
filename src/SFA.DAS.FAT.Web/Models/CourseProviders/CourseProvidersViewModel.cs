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
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public List<string> SelectedDeliveryModes { get; set; } = [];
    public List<string> SelectedEmployerApprovalRatings { get; set; } = [];
    public List<string> SelectedApprenticeApprovalRatings { get; set; } = [];
    public List<string> SelectedQarRatings { get; set; } = [];
    public string QarPeriod { get; set; }
    public string ReviewPeriod { get; set; }
    public string QarPeriodStartYear { get => $"20{QarPeriod.AsSpan(0, 2)}"; }
    public string QarPeriodEndYear { get => $"20{QarPeriod.AsSpan(2, 2)}"; }
    public string ReviewPeriodStartYear { get => $"20{ReviewPeriod.AsSpan(0, 2)}"; }
    public string ReviewPeriodEndYear { get => $"20{ReviewPeriod.AsSpan(2, 2)}"; }
    public string ProviderReviewsHeading { get => $"Provider reviews in {ReviewPeriodStartYear} to {ReviewPeriodEndYear}"; }
    public string CourseAchievementRateHeading { get => $"Course achievement rate in {QarPeriodStartYear} to {QarPeriodEndYear}"; }
    public List<CoursesProviderViewModel> Providers { get; set; }
    public List<ProviderOrderByOptionViewModel> ProviderOrderOptions { get; set; } = [];
    public PaginationViewModel Pagination { get; set; }
    public int TotalCount { get; set; }
    public string TotalMessage => GetTotalMessage();

    private string GetTotalMessage()
    {
        var totalToUse = TotalCount <= 0 ? "No" : TotalCount.ToString();

        var totalMessage = $"{totalToUse} result{(totalToUse != "1" ? "s" : "")}";

        if (!string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(Distance) &&
            Distance != DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
        {
            totalMessage = $"{totalMessage} within {Distance} miles";
        }

        return totalMessage;
    }

    private static List<DeliveryModeOptionViewModel> BuildDeliveryModeOptionViewModel()
    {
        var deliveryModeOptionViewModels = new List<DeliveryModeOptionViewModel>();

        foreach (ProviderDeliveryMode deliveryModeChoice in Enum.GetValues(typeof(ProviderDeliveryMode)))
        {
            deliveryModeOptionViewModels.Add(new DeliveryModeOptionViewModel
            {
                DeliveryItemChoice = deliveryModeChoice,
                Description = deliveryModeChoice.GetDescription(),
                Selected = false
            });
        }
        return deliveryModeOptionViewModels;
    }

    private static List<QarOptionViewModel> BuildQarRatingsViewModel()
    {
        var qarOptionViewModels = new List<QarOptionViewModel>();

        foreach (QarRating rating in Enum.GetValues(typeof(QarRating)))
        {
            qarOptionViewModels.Add(new QarOptionViewModel()
            {
                QarRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            });
        }
        return qarOptionViewModels;
    }

    private static List<EmployerProviderRatingOptionViewModel> BuildEmployerProviderRatingsViewModel()
    {
        var ratingViewModels = new List<EmployerProviderRatingOptionViewModel>();

        foreach (ProviderRating rating in Enum.GetValues(typeof(ProviderRating)))
        {
            ratingViewModels.Add(new EmployerProviderRatingOptionViewModel()
            {
                ProviderRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            });
        }
        return ratingViewModels;
    }

    private static List<ApprenticeProviderRatingOptionViewModel> BuildApprenticeProviderRatingsViewModel()
    {
        var ratingViewModels = new List<ApprenticeProviderRatingOptionViewModel>();

        foreach (ProviderRating rating in Enum.GetValues(typeof(ProviderRating)))
        {
            ratingViewModels.Add(new ApprenticeProviderRatingOptionViewModel()
            {
                ProviderRatingType = rating,
                Description = rating.GetDescription(),
                Selected = false
            });
        }
        return ratingViewModels;
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

    public FiltersViewModel CreateFilterSections()
    {
        var selectedFilterSections = CreateSelectedFilterSections();

        return new FiltersViewModel()
        {
            Route = RouteNames.CourseProviders,
            FilterSections =
            [
                CreateSearchFilterSection("search-location", LOCATION_SECTION_HEADING, LOCATION_SECTION_SUB_HEADING, nameof(Location), Location),
                CreateDropdownFilterSection("distance-filter", nameof(Distance), DISTANCE_SECTION_HEADING, DISTANCE_SECTION_SUB_HEADING, GetDistanceFilterValues(Distance)),
                CreateCheckboxListFilterSection("modes-filter", nameof(FilterType.DeliveryModes), DELIVERYMODES_SECTION_HEADING, DELIVERYMODES_SECTION_SUB_HEADING, GenerateDeliveryModesFilterItems()),
                CreateAccordionGroupFilterSection(
                    "ratings-select",
                    nameof(FilterType.Reviews),
                    [
                        CreateCheckboxListFilterSection("employer-ratings-filter", nameof(FilterType.EmployerProviderRatings), EMPLOYER_REVIEWS_SECTION_HEADING,null, GenerateEmployerReviewsFilterItems()),
                        CreateCheckboxListFilterSection("apprentice-ratings-filter", nameof(FilterType.ApprenticeProviderRatings), APPRENTICE_REVIEWS_SECTION_HEADING,null, GenerateApprenticeReviewsFilterItems())
                    ],
                    REVIEW_SECTION_HEADING,
                    $"From {ReviewPeriodStartYear} to {ReviewPeriodEndYear}"
                ),
                CreateAccordionFilterSection(
                    "qar-select",
                    string.Empty,
                    [
                        CreateCheckboxListFilterSection(
                            "qar-filter",
                            nameof(FilterType.QarRatings),
                            QAR_SECTION_HEADING,
                            $"From {QarPeriodStartYear} to {QarPeriodEndYear}",
                            GenerateQarFilterItems()
                        )
                    ])
                ],
            ClearFilterSections = selectedFilterSections
        };
    }

    private List<FilterItemViewModel> GenerateDeliveryModesFilterItems()
    {
        var deliveryModes = BuildDeliveryModeOptionViewModel();

        return deliveryModes?.Select(deliveryMode => new FilterItemViewModel
        {
            Value = deliveryMode.DeliveryItemChoice.ToString(),
            DisplayText = deliveryMode.DeliveryItemChoice.GetDescription(),
            DisplayDescription = deliveryMode.DeliveryItemChoice switch
            {
                ProviderDeliveryMode.Online => FilterService.DELIVERYMODES_SECTION_ONLINE_DISPLAYDESCRIPTION,
                ProviderDeliveryMode.Workplace => FilterService.DELIVERYMODES_SECTION_WORKPLACE_DISPLAYDESCRIPTION,
                ProviderDeliveryMode.Provider => FilterService.DELIVERYMODES_SECTION_PROVIDER_DISPLAYDESCRIPTION,
                _ => string.Empty
            },
            IsSelected = SelectedDeliveryModes?.Contains(deliveryMode.DeliveryItemChoice.ToString()) ?? false
        })
            .ToList() ?? [];
    }

    private List<FilterItemViewModel> GenerateQarFilterItems()
    {
        var qarRatings = BuildQarRatingsViewModel();

        return qarRatings?.Select(qarRating => new FilterItemViewModel
        {
            Value = qarRating.QarRatingType.ToString(),
            DisplayText = qarRating.QarRatingType.GetDescription(),
            IsSelected = SelectedQarRatings?.Contains(qarRating.QarRatingType.ToString()) ?? false
        })
            .ToList() ?? [];
    }

    private List<FilterItemViewModel> GenerateEmployerReviewsFilterItems()
    {
        var ratings = BuildEmployerProviderRatingsViewModel();

        return ratings?.Select(rating => new FilterItemViewModel
        {
            Value = rating.ProviderRatingType.ToString(),
            DisplayText =
                rating.ProviderRatingType == ProviderRating.NotYetReviewed
                ? "No employer reviews"
                : rating.ProviderRatingType.GetDescription(),
            IsSelected = SelectedEmployerApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
        })
            .ToList() ?? [];
    }

    private List<FilterItemViewModel> GenerateApprenticeReviewsFilterItems()
    {
        var ratings = BuildApprenticeProviderRatingsViewModel();

        return ratings?.Select(rating => new FilterItemViewModel
        {
            Value = rating.ProviderRatingType.ToString(),
            DisplayText =
                rating.ProviderRatingType == ProviderRating.NotYetReviewed
                    ? "No apprentice reviews"
                    : rating.ProviderRatingType.GetDescription(),
            IsSelected = SelectedApprenticeApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
        })
            .ToList() ?? [];
    }

    private IReadOnlyList<ClearFilterSectionViewModel> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();

        AddLocationAndDistanceFilters(selectedFilters);
        AddDeliveryModesFilter(selectedFilters);
        AddRatingFilters(selectedFilters);
        AddSelectedFilter(selectedFilters, FilterType.OrderBy, OrderBy.ToString());

        if (selectedFilters.Count == 0)
        {
            return [];
        }

        return CreateClearFilterSections(
                selectedFilters,
                _valueFunctions,
                [FilterType.Distance, FilterType.OrderBy]
            );
    }

    private void AddLocationAndDistanceFilters(Dictionary<FilterType, List<string>> selectedFilters)
    {
        AddSelectedFilter(selectedFilters, FilterType.Location, Location);

        if (!selectedFilters.ContainsKey(FilterType.Location) && string.IsNullOrEmpty(Distance))
        {
            Distance = DistanceService.TEN_MILES.ToString();
        }

        if (DistanceService.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
        }
    }

    private void AddDeliveryModesFilter(Dictionary<FilterType, List<string>> selectedFilters)
    {
        if (SelectedDeliveryModes?.Count > 0 && SelectedDeliveryModes.Count > 0)
        {
            var deliveryModes = GenerateDeliveryModesFilterItems();
            var selectedDeliveryNames = deliveryModes
                .Where(dm => SelectedDeliveryModes.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            if (CourseType == CourseType.ShortCourse && selectedDeliveryNames is { Count: > 0 })
            {
                selectedFilters[FilterType.DeliveryModes] = selectedDeliveryNames;
            }
            else
            {
                AddSelectedFilter(selectedFilters, FilterType.DeliveryModes, selectedDeliveryNames);
            }
        }
    }

    private void AddRatingFilters(Dictionary<FilterType, List<string>> selectedFilters)
    {
        AddRatingFilter(selectedFilters, SelectedEmployerApprovalRatings, GenerateEmployerReviewsFilterItems, FilterType.EmployerProviderRatings);
        AddRatingFilter(selectedFilters, SelectedApprenticeApprovalRatings, GenerateApprenticeReviewsFilterItems, FilterType.ApprenticeProviderRatings);
        AddRatingFilter(selectedFilters, SelectedQarRatings, GenerateQarFilterItems, FilterType.QarRatings);
    }

    private void AddRatingFilter(Dictionary<FilterType, List<string>> selectedFilters, List<string>? selectedRatings, Func<List<FilterItemViewModel>> generateFilterItems, FilterType filterType)
    {
        if (selectedRatings?.Count > 0)
        {
            var filterItems = generateFilterItems();
            var selectedItems = filterItems
                .Where(item => selectedRatings.Contains(item.Value.ToString()))
                .Select(item => item.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, filterType, selectedItems);
        }
    }

    private readonly string _requestApprenticeshipTrainingUrl;
    private readonly string _employerAccountsUrl;

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

    public string GetHelpFindingCourseUrl(string larsCode)
    {
        var redirectUri = $"{_requestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={larsCode}&requestType={EntryPoint.CourseDetail}";

        var locationQueryParam = !string.IsNullOrEmpty(Location) ? $"&location={Location}" : string.Empty;

        return $"{_employerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    private readonly Dictionary<FilterType, Func<string, string>> _valueFunctions;

    private static string GetDeliveryModeValue(string filterValue)
    {
        var deliveryModes = BuildDeliveryModeOptionViewModel();
        foreach (var dm in deliveryModes)
        {
            if (dm.Description == filterValue)
            {
                return dm.DeliveryItemChoice.ToString();
            }
        }

        return filterValue;
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
                            : ValueTuple.Create(nameof(Distance), DistanceService.ACROSS_ENGLAND_FILTER_VALUE));
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

    private static string GetQarRatingsValue(string filterValue)
    {
        var qars = BuildQarRatingsViewModel();
        foreach (var qar in qars)
        {
            if (qar.Description == filterValue)
            {
                return qar.QarRatingType.ToString();
            }
        }

        return filterValue;
    }

    private static string GetProviderRatingsValue(string filterValue)
    {
        var ratings = BuildEmployerProviderRatingsViewModel();
        foreach (var rating in ratings)
        {
            if (rating.Description == filterValue)
            {
                return rating.ProviderRatingType.ToString();
            }
        }

        return filterValue;
    }
}
