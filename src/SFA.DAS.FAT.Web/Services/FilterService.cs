using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.Services;

public static class FilterService
{
    public enum FilterComponentType
    {
        CheckboxList,
        Dropdown,
        TextBox,
        Search,
        Accordion,
        AccordionGroup
    }

    public enum FilterType
    {
        Location,
        Levels,
        Categories,
        KeyWord,
        OrderBy,
        Distance,
        DeliveryModes,
        QarRatings,
        EmployerProviderRatings,
        ApprenticeProviderRatings,
        Reviews,
        ApprenticeshipTypes
    }

    public const string KEYWORD_SECTION_HEADING = "Course";
    public const string KEYWORD_SECTION_SUB_HEADING = "Enter course, job or standard";

    public const string LOCATION_SECTION_HEADING = "Apprentice's work location";
    public const string LOCATION_SECTION_SUB_HEADING = "Enter city or postcode";

    public const string DISTANCE_SECTION_HEADING = "Apprentice can travel";
    public const string DISTANCE_SECTION_SUB_HEADING = "Distance apprentice can travel";

    public const string LEVELS_SECTION_HEADING = "Apprenticeship level";
    public const string CATEGORIES_SECTION_HEADING = "Job categories";

    public const string LEVEL_INFORMATION_DISPLAY_TEXT = "What apprenticeship levels mean (opens in new tab or window)";
    public const string LEVEL_INFORMATION_URL = "https://www.gov.uk/what-different-qualification-levels-mean/list-of-qualification-levels";

    public const string ACROSS_ENGLAND_FILTER_TEXT = "Across England";


    public const string DELIVERYMODES_SECTION_HEADING = "Training options";
    public const string DELIVERYMODES_SECTION_SUB_HEADING = "Select a training option";

    public const string APPRENTICESHIP_TYPES_SECTION_HEADING = "Training type";

    public const string QAR_SECTION_HEADING = "Achievement rate";

    public const string REVIEW_SECTION_HEADING = "Reviews";

    public const string EMPLOYER_REVIEWS_SECTION_HEADING = "Average employer review";
    public const string APPRENTICE_REVIEWS_SECTION_HEADING = "Average apprentice review";

    public static Dictionary<FilterType, string> ClearFilterSectionHeadings => _ClearFilterSectionHeadings;

    private static readonly Dictionary<FilterType, string> _ClearFilterSectionHeadings = new Dictionary<FilterType, string>()
    {
        { FilterType.KeyWord, KEYWORD_SECTION_HEADING },
        { FilterType.Location, LOCATION_SECTION_HEADING},
        { FilterType.Levels, LEVELS_SECTION_HEADING },
        { FilterType.Categories, CATEGORIES_SECTION_HEADING },
        { FilterType.DeliveryModes, DELIVERYMODES_SECTION_HEADING },
        { FilterType.QarRatings, QAR_SECTION_HEADING },
        { FilterType.EmployerProviderRatings, EMPLOYER_REVIEWS_SECTION_HEADING},
        { FilterType.ApprenticeProviderRatings, APPRENTICE_REVIEWS_SECTION_HEADING},
        { FilterType.ApprenticeshipTypes, APPRENTICESHIP_TYPES_SECTION_HEADING}
    };

    public static FilterSection CreateInputFilterSection(string id, string heading, string subHeading, string filterFor, string inputValue)
    {
        return new TextBoxFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            InputValue = inputValue ?? string.Empty
        };
    }

    public static FilterSection CreateSearchFilterSection(string id, string heading, string subHeading, string filterFor, string inputValue)
    {
        return new SearchFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            InputValue = inputValue
        };
    }

    public static FilterSection CreateDropdownFilterSection(string id, string filterFor, string heading, string subHeading, List<FilterItemViewModel> items)
    {
        return new DropdownFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            Items = items
        };
    }

    public static FilterSection CreateCheckboxListFilterSection(string id, string filterFor, string heading, string subHeading, List<FilterItemViewModel> items, string linkDisplayText = "", string linkDisplayUrl = "")
    {
        CheckboxListFilterSectionViewModel section = new()
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            Items = items
        };

        if (!string.IsNullOrWhiteSpace(linkDisplayText) && !string.IsNullOrWhiteSpace(linkDisplayUrl))
        {
            section.Link = new SectionLinkViewModel
            {
                DisplayText = linkDisplayText,
                Url = linkDisplayUrl
            };
        }

        return section;
    }

    public static FilterSection CreateAccordionFilterSection(string id, string sectionFor, List<FilterSection> children, string heading = "", string subHeading = "")
    {
        return new AccordionFilterSectionViewModel
        {
            Id = id,
            For = sectionFor,
            Children = children,
            Heading = heading,
            SubHeading = subHeading
        };
    }

    public static FilterSection CreateAccordionGroupFilterSection(string id, string sectionFor, List<FilterSection> children, string heading = "", string subHeading = "")
    {
        return new AccordionGroupFilterSectionViewModel
        {
            Id = id,
            For = sectionFor,
            Children = children,
            Heading = heading,
            SubHeading = subHeading
        };
    }

    private static Dictionary<FilterType, FilterType[]> LinkedFilters = new Dictionary<FilterType, FilterType[]>()
    {
        { FilterType.Location, [FilterType.Distance] }
    };

    public static IReadOnlyList<ClearFilterSectionViewModel> CreateClearFilterSections(
        Dictionary<FilterType, List<string>> selectedFilters,
        Dictionary<FilterType, Func<string, string>> overrideValueFunctions = null,
        FilterType[] excludedFilterTypes = null
    )
    {
        if (selectedFilters == null || selectedFilters.Count == 0)
        {
            return Array.Empty<ClearFilterSectionViewModel>();
        }

        List<ClearFilterSectionViewModel> clearFilterSections = [];

        foreach (var filter in selectedFilters)
        {
            if (excludedFilterTypes is not null && excludedFilterTypes.Contains(filter.Key) || filter.Value.Count == 0)
            {
                continue;
            }

            clearFilterSections.Add(new ClearFilterSectionViewModel
            {
                FilterType = filter.Key,
                Title = ClearFilterSectionHeadings[filter.Key],
                Items = filter.Value.Select(value => new ClearFilterItemViewModel
                {
                    DisplayText = GetDisplayValue(filter.Key, value, selectedFilters),
                    ClearLink = BuildQueryWithoutValue(filter.Key, value, selectedFilters, overrideValueFunctions)
                })
                .ToList()
            });
        }

        return clearFilterSections;
    }

    public static List<FilterItemViewModel> GetDistanceFilterValues(string selectedDistance)
    {
        var validDistance = DistanceService.GetValidDistance(selectedDistance);

        List<FilterItemViewModel> distanceFilterItems = [];

        distanceFilterItems.AddRange(DistanceService.Distances.Select(distance => new FilterItemViewModel
        {
            Value = distance.ToString(),
            DisplayText = $"{distance} miles",
            IsSelected = validDistance == distance
        }));

        distanceFilterItems.Add(new FilterItemViewModel
        {
            Value = DistanceService.ACROSS_ENGLAND_FILTER_VALUE,
            DisplayText = ACROSS_ENGLAND_FILTER_TEXT,
            IsSelected =
                string.Equals(
                    selectedDistance,
                    DistanceService.ACROSS_ENGLAND_FILTER_VALUE,
                    StringComparison.OrdinalIgnoreCase
                ) || validDistance == DistanceService.DEFAULT_DISTANCE
        });

        return distanceFilterItems;
    }

    private static string BuildQueryWithoutValue(
        FilterType filterType,
        string value,
        Dictionary<FilterType, List<string>> queryParams,
        Dictionary<FilterType, Func<string, string>> overrideValueFunctions = null
    )
    {
        if (queryParams == null || queryParams.Count == 0)
        {
            return string.Empty;
        }

        var queryBuilder = new StringBuilder();

        foreach (var param in queryParams)
        {
            if (param.Value is null || param.Value.Count < 1)
            {
                continue;
            }

            var overrideValueFunction = overrideValueFunctions?.TryGetValue(param.Key, out var func) == true ? func : null;

            AppendQueryParameters(filterType, value, param, queryBuilder, overrideValueFunction);
        }

        return queryBuilder.Length > 0 ? queryBuilder.ToString() : string.Empty;
    }

    private static void AppendQueryParameters(FilterType filterType, string value, KeyValuePair<FilterType, List<string>> param,
        StringBuilder queryBuilder, Func<string, string> overrideValueFunction)
    {
        if (param.Key == filterType)
        {
            foreach (var val in param.Value.Where(v => v != value))
            {
                var paramValue = GetClearVal(val, filterType, param.Key);

                if (string.IsNullOrWhiteSpace(paramValue))
                {
                    continue;
                }

                AppendQueryParam(queryBuilder, param.Key, paramValue, overrideValueFunction);
            }
        }
        else
        {
            foreach (var val in param.Value)
            {
                var paramValue = GetClearVal(val, filterType, param.Key);

                if (string.IsNullOrWhiteSpace(paramValue))
                {
                    continue;
                }

                AppendQueryParam(queryBuilder, param.Key, paramValue, overrideValueFunction);
            }
        }
    }

    private static string GetClearVal(string currentVal, FilterType filterType, FilterType queryParamType)
    {
        if (!LinkedFilters.TryGetValue(filterType, out var linkedFilters) || linkedFilters is null)
        {
            linkedFilters = Array.Empty<FilterType>();
        }

        var isParamLinked = linkedFilters.Contains(queryParamType);

        if (isParamLinked)
        {
            return null;
        }

        return currentVal;
    }

    private static void AppendQueryParam(StringBuilder builder, FilterType key, string value, Func<string, string> overrideValueFunction = null)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var queryValue = overrideValueFunction == null
                ? value
                : overrideValueFunction(value);

            builder
                .Append(builder.Length > 0 ? '&' : '?')
                .Append(key.ToString().ToLowerInvariant())
                .Append('=')
                .Append(queryValue);
        }
    }

    public static void AddSelectedFilter(Dictionary<FilterType, List<string>> filters, FilterType filterType, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            filters[filterType] = [value];
        }
    }

    public static void AddSelectedFilter(Dictionary<FilterType, List<string>> filters, FilterType filterType, List<string> values)
    {
        var valuesToProcess = values;

        if (filterType == FilterType.DeliveryModes)
        {
            valuesToProcess = values?.Where(x => x != ProviderDeliveryMode.Provider.GetDescription()).ToList();
        }

        if (valuesToProcess is { Count: > 0 })
        {
            filters[filterType] = valuesToProcess;
        }
    }

    private static string GetDisplayValue(FilterType key, string displayValue, Dictionary<FilterType, List<string>> queryParams)
    {
        return key switch
        {
            FilterType.Location => GetWorkLocationDistanceDisplayMessage(queryParams),
            _ => displayValue
        };
    }

    private static string GetWorkLocationDistanceDisplayMessage(Dictionary<FilterType, List<string>> queryParams)
    {
        if (!queryParams.TryGetValue(FilterType.Location, out var location) || location == null || location.Count == 0 || string.IsNullOrWhiteSpace(location[0]))
        {
            return string.Empty;
        }

        if (!queryParams.TryGetValue(FilterType.Distance, out var distanceList) || distanceList == null || distanceList.Count == 0)
        {
            return $"{location[0]} ({ACROSS_ENGLAND_FILTER_TEXT})";
        }

        if (!int.TryParse(distanceList[0], out var distance) || distance < 1 || distance > 100)
        {
            return $"{location[0]} ({ACROSS_ENGLAND_FILTER_TEXT})";
        }

        return $"{location[0]} (within {distance} miles)";
    }
}
