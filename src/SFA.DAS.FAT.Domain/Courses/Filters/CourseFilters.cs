using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses.Filters;

public static class CourseFilters
{
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
    public const string LOCATION_SECTION_HEADING = "Apprentice's work location";
    public const string LEVELS_SECTION_HEADING = "Qualification level";
    public const string CATEGORIES_SECTION_HEADING = "Apprenticeship category";

    public static Dictionary<FilterType, string> ClearFilterSectionHeadings => _ClearFilterSectionHeadings;

    private static readonly Dictionary<FilterType, string> _ClearFilterSectionHeadings = new Dictionary<FilterType, string>()
    {
        { FilterType.KeyWord, KEYWORD_SECTION_HEADING },
        { FilterType.Location,  LOCATION_SECTION_HEADING},
        { FilterType.Levels, LEVELS_SECTION_HEADING },
        { FilterType.Categories, CATEGORIES_SECTION_HEADING  }
    };
}

