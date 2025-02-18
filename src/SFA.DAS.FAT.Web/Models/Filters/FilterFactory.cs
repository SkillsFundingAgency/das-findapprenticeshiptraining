using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.Models.Filters;

public static class FilterFactory
{
    public enum FilterComponentType
    {
        CheckboxList,
        Dropdown,
        TextBox,
        Search,
        Accordion
    }

    public enum FilterType
    {
        Location,
        Levels,
        Categories,
        KeyWord,
        OrderBy,
        Distance
    }

    public const string KEYWORD_SECTION_HEADING = "Course";
    public const string KEYWORD_SECTION_SUB_HEADING = "Enter course, job or standard";

    public const string LOCATION_SECTION_HEADING = "Apprentice's work location";
    public const string LOCATION_SECTION_SUB_HEADING = "Enter city or postcode";

    public const string DISTANCE_SECTION_HEADING = "Apprentice can travel";
    public const string DISTANCE_SECTION_SUB_HEADING = "Distance apprentice can travel to training";

    public const string LEVELS_SECTION_HEADING = "Apprenticeship level";
    public const string CATEGORIES_SECTION_HEADING = "Job Categories";

    public const string LEVEL_INFORMATION_DISPLAY_TEXT = "What qualification levels mean (opens in new tab or window)";
    public const string LEVEL_INFORMATION_URL = "https://www.gov.uk/what-different-qualification-levels-mean/list-of-qualification-levels";

    public const string ACROSS_ENGLAND_FILTER_TEXT = "Across England";

    public static Dictionary<FilterType, string> ClearFilterSectionHeadings => _ClearFilterSectionHeadings;

    private static readonly Dictionary<FilterType, string> _ClearFilterSectionHeadings = new Dictionary<FilterType, string>()
    {
        { FilterType.KeyWord, KEYWORD_SECTION_HEADING },
        { FilterType.Location, LOCATION_SECTION_HEADING},
        { FilterType.Levels, LEVELS_SECTION_HEADING },
        { FilterType.Categories, CATEGORIES_SECTION_HEADING }
    };

    public static FilterSection CreateInputFilterSection(string id, string heading, string subHeading, string filterFor, string inputValue)
    {
        return new TextBoxFilterSectionViewModel
        {
            Id = id,
            For = filterFor,
            Heading = heading,
            SubHeading = subHeading,
            InputValue = inputValue
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

    public static FilterSection CreateCheckboxListFilterSection(string id, string filterFor, string heading, List<FilterItemViewModel> items, string linkDisplayText = "", string linkDisplayUrl = "")
    {
        CheckboxListFilterSectionViewModel section = new CheckboxListFilterSectionViewModel()
        {
            Id = id,
            For = filterFor,
            Heading = heading,
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

    public static FilterSection CreateAccordionFilterSection(string id, string sectionFor, List<FilterSection> children)
    {
        return new AccordionFilterSectionViewModel
        {
            Id = id,
            For = sectionFor,
            Children = children
        };
    }

    private static Dictionary<FilterType, FilterType[]> LinkedFilters = new Dictionary<FilterType, FilterType[]>()
    {
        { FilterType.Location, [FilterType.Distance] }
    };

    public static IReadOnlyList<ClearFilterSectionViewModel> CreateClearFilterSections(
        Dictionary<FilterType, List<string>>? selectedFilters,
        Dictionary<FilterType, Func<string, string>>? overrideValueFunctions = null,
        FilterType[]? excludedFilterTypes = null
    )
    {
        if (selectedFilters == null || selectedFilters.Count == 0)
        {
            return Array.Empty<ClearFilterSectionViewModel>();
        }

        List<ClearFilterSectionViewModel> clearFilterSections = new List<ClearFilterSectionViewModel>();

        foreach(KeyValuePair<FilterType, List<string>> filter in selectedFilters)
        {
            if((excludedFilterTypes is not null && excludedFilterTypes.Contains(filter.Key)) || filter.Value.Count == 0)
            {
                continue;
            }

            clearFilterSections.Add(new ClearFilterSectionViewModel()
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

    public static List<FilterItemViewModel> GetDistanceFilterValues(string? selectedDistance)
    {
        var distanceFilterItems = ValidDistances.Distances
            .Select(distance => new FilterItemViewModel
            {
                Value = distance.ToString(),
                DisplayText = $"{distance} Miles",
                Selected = ValidDistances.GetValidDistance(selectedDistance) == distance
            })
        .ToList();

        distanceFilterItems.Add(new FilterItemViewModel
        {
            Value = ValidDistances.ACROSS_ENGLAND_FILTER_VALUE,
            DisplayText = ACROSS_ENGLAND_FILTER_TEXT,
            Selected = selectedDistance == ValidDistances.ACROSS_ENGLAND_FILTER_VALUE
        });

        return distanceFilterItems!;
    }

    private static string BuildQueryWithoutValue(
        FilterType filterType,
        string value, 
        Dictionary<FilterType, List<string>> queryParams, 
        Dictionary<FilterType, Func<string, string>>? overrideValueFunctions = null
    )
    {
        if (queryParams == null || queryParams.Count == 0)
        {
            return string.Empty;
        }

        var queryBuilder = new StringBuilder();

        foreach (var param in queryParams)
        {
            if (param.Value == null || param.Value.Count == 0)
            {
                continue;
            }

            Func<string, string>? overrideValueFunction = (overrideValueFunctions != null && overrideValueFunctions.TryGetValue(param.Key, out var func)) ? func : null;

            if (param.Key == filterType)
            {
                foreach (var val in param.Value.Where(v => v != value))
                {
                    string? paramValue = GetClearVal(val, filterType, param.Key);

                    if(string.IsNullOrWhiteSpace(paramValue))
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
                    string? paramValue = GetClearVal(val, filterType, param.Key);

                    if (string.IsNullOrWhiteSpace(paramValue))
                    {
                        continue;
                    }

                    AppendQueryParam(queryBuilder, param.Key, paramValue, overrideValueFunction);
                }
            }
        }

        return queryBuilder.ToString();
    }

    private static string? GetClearVal(string currentVal, FilterType filterType, FilterType queryParamType)
    {
        if (!LinkedFilters.TryGetValue(filterType, out FilterType[]? linkedFilters) || linkedFilters is null)
        {
            linkedFilters = Array.Empty<FilterType>();
        }

        var isParamLinked = linkedFilters.Contains(queryParamType);

        if (!isParamLinked)
        {
            return currentVal;
        }
        else
        {
            return null;
        }
    }

    private static void AppendQueryParam(StringBuilder builder, FilterType key, string value, Func<string, string>? overrideValueFunction = null)
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

            string queryValue = overrideValueFunction == null ? value : GetQueryValue(key, value, overrideValueFunction);

            builder.Append($"{key.ToString().ToLower()}={queryValue}");
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
        if (values?.Count > 0)
        {
            filters[filterType] = values;
        }
    }

    private static string GetQueryValue(FilterType key, string filterValue, Func<string, string> overrideValueFunction)
    {
        return key switch
        {
            FilterType.Levels => overrideValueFunction(filterValue),
            _ => filterValue
        };
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

        if (!int.TryParse(distanceList[0], out int distance) || distance < 1 || distance > 100)
        {
            return $"{location[0]} ({ACROSS_ENGLAND_FILTER_TEXT})";
        }

        return $"{location[0]} (within {distance} miles)";
    }
}
