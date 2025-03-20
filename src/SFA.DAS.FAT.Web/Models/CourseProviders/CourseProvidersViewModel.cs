using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Services;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class CourseProvidersViewModel : PageLinksViewModelBase
{
    public int Id { get; set; }
    public ProviderOrderBy OrderBy { get; set; }
    public string CourseTitleAndLevel { get; set; } = string.Empty;

    public string Distance { get; set; } = "All";

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


    public List<ProviderOrderByOptionViewModel> ProviderOrderDropdown { get => GenerateProviderOrderDropdown(); }

    private List<ProviderOrderByOptionViewModel> GenerateProviderOrderDropdown()
    {
        var dropdown = new List<ProviderOrderByOptionViewModel>();

        foreach (ProviderOrderBy orderByChoice in Enum.GetValues(typeof(ProviderOrderBy)))
        {
            dropdown.Add(new ProviderOrderByOptionViewModel
            {
                ProviderOrderBy = orderByChoice,
                Description = orderByChoice.GetDescription(),
                Selected = orderByChoice == OrderBy
            });
        }

        return dropdown;
    }

    public int TotalCount { get; set; }

    public string TotalMessage => GetTotalMessage();

    private string GetTotalMessage()
    {
        var totalToUse = TotalCount <= 0 ? "No" : TotalCount.ToString();

        var totalMessage = $"{totalToUse} result{(totalToUse != "1" ? "s" : "")}";

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
            CreateCheckboxListFilterSection("qar-filter", nameof(FilterType.QarRatings), QAR_SECTION_HEADING, $"From {QarPeriodStartYear} to {QarPeriodEndYear}", GenerateQarFilterItems())
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
            Selected = SelectedDeliveryModes?.Contains(deliveryMode.DeliveryItemChoice.ToString()) ?? false
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
            Selected = SelectedQarRatings?.Contains(qarRating.QarRatingType.ToString()) ?? false
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
            Selected = SelectedEmployerApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
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
            Selected = SelectedApprenticeApprovalRatings?.Contains(rating.ProviderRatingType.ToString()) ?? false
        })
            .ToList() ?? [];
    }

    private IReadOnlyList<ClearFilterSectionViewModel> CreateSelectedFilterSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();

        AddSelectedFilter(selectedFilters, FilterType.Location, Location);

        if (!selectedFilters.ContainsKey(FilterType.Location) && string.IsNullOrEmpty(Distance))
        {
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE;
        }

        if (DistanceService.IsValidDistance(Distance))
        {
            AddSelectedFilter(selectedFilters, FilterType.Distance, Distance);
        }


        if (SelectedDeliveryModes?.Count > 0 && SelectedDeliveryModes.Count > 0)
        {
            var deliveryModes = GenerateDeliveryModesFilterItems();
            var selectedDeliveryNames = deliveryModes
                .Where(dm => SelectedDeliveryModes.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.DeliveryModes, selectedDeliveryNames);
        }

        if (SelectedEmployerApprovalRatings?.Count > 0 && SelectedEmployerApprovalRatings.Count > 0)
        {
            var reviews = GenerateEmployerReviewsFilterItems();
            var selectedReviews = reviews
                .Where(dm => SelectedEmployerApprovalRatings.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.EmployerProviderRatings, selectedReviews);
        }

        if (SelectedApprenticeApprovalRatings?.Count > 0 && SelectedApprenticeApprovalRatings.Count > 0)
        {
            var reviews = GenerateApprenticeReviewsFilterItems();
            var selectedReviews = reviews
                .Where(dm => SelectedApprenticeApprovalRatings.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.ApprenticeProviderRatings, selectedReviews);
        }

        if (SelectedQarRatings?.Count > 0 && SelectedQarRatings.Count > 0)
        {
            var qars = GenerateQarFilterItems();
            var selectedQars = qars
                .Where(dm => SelectedQarRatings.Contains(dm.Value.ToString()))
                .Select(dm => dm.DisplayText)
                .ToList();

            AddSelectedFilter(selectedFilters, FilterType.QarRatings, selectedQars);
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

    private readonly IUrlHelper _urlHelper;

    private readonly string _requestApprenticeshipTrainingUrl;
    private readonly string _employerAccountsUrl;

    public CourseProvidersViewModel(FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration, IUrlHelper urlHelper)
    {

        _urlHelper = urlHelper;
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

    public string GetHelpFindingCourseUrl(int larsCode)
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
