using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.Services;

public static class FilterService
{

    public const string KeywordSectionHeading = "Course";
    public const string KeywordSectionSubHeading = "Enter course, job or standard";

    public const string LocationSectionHeading = "Learner's work location";
    public const string LocationSectionSubHeading = "Enter city or postcode";

    public const string DistanceSectionHeading = "Learner can travel";
    public const string DistanceSectionSubHeading = "Distance learner can travel";

    public const string LevelsSectionHeading = "Training level";
    public const string CategoriesSectionHeading = "Job categories";

    public const string LevelInformationDisplayText = "Find out more about training levels (opens in new tab)";
    public const string LevelInformationUrl = "https://www.gov.uk/what-different-qualification-levels-mean/list-of-qualification-levels";

    public const string AcrossEnglandFilterText = "Across England";


    public const string DeliveryModesSectionHeading = "Training options";
    public const string DeliveryModesSectionSubHeading = "Select a training option";
    public const string DeliveryModesSectionOnlineDisplayDescription = "Your learner will complete this training remotely.";
    public const string DeliveryModesSectionWorkplaceDisplayDescription = "The training provider will travel to you to deliver this course.";
    public const string DeliveryModesSectionProviderDisplayDescription = "Your learner will travel to the training provider to complete this course.";

    public const string TrainingTypesSectionHeading = "Training type";

    public const string QarSectionHeading = "Achievement rate";

    public const string ReviewSectionHeading = "Reviews";

    public const string EmployerReviewsSectionHeading = "Average employer review";
    public const string ApprenticeReviewsSectionHeading = "Average apprentice review";

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
        LearningTypes
    }

    public static Dictionary<FilterType, string> ClearFilterSectionHeadings => _ClearFilterSectionHeadings;

    private static readonly Dictionary<FilterType, string> _ClearFilterSectionHeadings = new Dictionary<FilterType, string>()
    {
        { FilterType.KeyWord, KeywordSectionHeading },
        { FilterType.Location, LocationSectionHeading},
        { FilterType.Levels, LevelsSectionHeading },
        { FilterType.Categories, CategoriesSectionHeading },
        { FilterType.DeliveryModes, DeliveryModesSectionHeading },
        { FilterType.QarRatings, QarSectionHeading },
        { FilterType.EmployerProviderRatings, EmployerReviewsSectionHeading},
        { FilterType.ApprenticeProviderRatings, ApprenticeReviewsSectionHeading},
        { FilterType.LearningTypes, TrainingTypesSectionHeading}
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
            Value = DistanceService.AcrossEnglandFilterValue,
            DisplayText = AcrossEnglandFilterText,
            IsSelected =
                string.Equals(
                    selectedDistance,
                    DistanceService.AcrossEnglandFilterValue,
                    StringComparison.OrdinalIgnoreCase
                ) || validDistance == DistanceService.DefaultDistance
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
            FilterType.LearningTypes => GetLearningTypeDisplayMessage(displayValue),
            _ => displayValue
        };
    }

    private static string GetLearningTypeDisplayMessage(string displayValue)
    {
        return Enum.TryParse<LearningType>(displayValue, ignoreCase: true, out var learningType)
            ? learningType.GetDescription()
            : string.Empty;
    }

    private static string GetWorkLocationDistanceDisplayMessage(Dictionary<FilterType, List<string>> queryParams)
    {
        if (!queryParams.TryGetValue(FilterType.Location, out var location) || location == null || location.Count == 0 || string.IsNullOrWhiteSpace(location[0]))
        {
            return string.Empty;
        }

        if (!queryParams.TryGetValue(FilterType.Distance, out var distanceList) || distanceList == null || distanceList.Count == 0)
        {
            return $"{location[0]} ({AcrossEnglandFilterText})";
        }

        if (!int.TryParse(distanceList[0], out var distance) || distance < 1 || distance > 100)
        {
            return $"{location[0]} ({AcrossEnglandFilterText})";
        }

        return $"{location[0]} (within {distance} miles)";
    }
}
