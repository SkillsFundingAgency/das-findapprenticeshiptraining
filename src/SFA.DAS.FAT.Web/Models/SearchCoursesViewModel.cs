using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.Models;

public class SearchCoursesViewModel : PageLinksViewModelBase
{
    public string CourseTerm { get; set; }
    public List<TypeViewModel> Types { get; set; } = [];
    public List<string> SelectedTypes { get; set; } = [];

    public const string APPRENTICESHIP_TYPE_APPRENTICESHIPUNITS_DESCRIPTION = "Short training courses based on existing apprenticeships, levels 2 to 7";
    public const string APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string APPRENTICESHIP_TYPE_STANDARD_DESCRIPTION = "Apprenticeships that qualify you for a job, levels 2 to 7";

    private List<FilterItemViewModel> GenerateApprenticeshipTypesFilterItems()
    {
        return Types?.Select(type => new FilterItemViewModel
        {
            Value = type.Name,
            DisplayText = type.Name,
            DisplayDescription =
                type.Name == ApprenticeshipType.ApprenticeshipUnits.GetDescription()
                    ? APPRENTICESHIP_TYPE_APPRENTICESHIPUNITS_DESCRIPTION
                    : type.Name == ApprenticeshipType.FoundationApprenticeship.GetDescription()
                    ? APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION
                    : APPRENTICESHIP_TYPE_STANDARD_DESCRIPTION,
            Selected = SelectedTypes?.Contains(type.Name) ?? false
        }).ToList() ?? [];
    }

    public CheckboxListFilterSectionViewModel TrainingTypesFilter => new()
    {
        Id = "training-types",
        Heading = "Training type",
        For = nameof(SelectedTypes),
        Items = GenerateApprenticeshipTypesFilterItems(),
        Link = null
    };
}
