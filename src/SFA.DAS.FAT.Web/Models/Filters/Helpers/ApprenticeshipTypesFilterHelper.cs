using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models.Filters.Helpers;

public static class ApprenticeshipTypesFilterHelper
{
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_UNITS_DESCRIPTION = "Short training courses based on existing apprenticeships, levels 2 to 7";
    public const string APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION = "Apprenticeships that qualify you for a job, levels 2 to 7";
    public static List<FilterItemViewModel> BuildItems(List<TypeViewModel> types, List<string> selectedTypes)
    {
        return types?.Select(type => new FilterItemViewModel
        {
            Value = type.Name,
            DisplayText = type.Name,
            DisplayDescription =
                type.Name == ApprenticeshipType.ApprenticeshipUnits.GetDescription()
                    ? APPRENTICESHIP_TYPE_APPRENTICESHIP_UNITS_DESCRIPTION
                    : type.Name == ApprenticeshipType.FoundationApprenticeship.GetDescription()
                        ? APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION
                        : APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION,
            Selected = selectedTypes?.Contains(type.Name) ?? false
        }).ToList() ?? [];
    }
}
