using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models;

public class SearchCoursesViewModel : PageLinksViewModelBase
{
    public string CourseTerm { get; set; }
    public List<string> SelectedTypes { get; set; } = [];
    public List<FilterItemViewModel> TrainingTypesFilterItems { get; set; } = [];
    public CheckboxListFilterSectionViewModel TrainingTypesCheckboxListItems => new()
    {
        Id = "training-types",
        Heading = "Training type",
        For = nameof(SelectedTypes),
        Items = TrainingTypesFilterItems,
        Link = null
    };
}
