using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
namespace SFA.DAS.FAT.Web.Models;

public class SearchCoursesViewModel : PageLinksViewModelBase
{
    public string CourseTerm { get; set; }
    public List<TypeViewModel> Types { get; set; } = [];
    public List<string> SelectedTypes { get; set; } = [];

    public CheckboxListFilterSectionViewModel TrainingTypesFilter => new()
    {
        Id = "training-types",
        Heading = "Training type",
        For = nameof(SelectedTypes),
        Items = ApprenticeshipTypesFilterHelper.BuildItems(Types, SelectedTypes),
        Link = null
    };
}
