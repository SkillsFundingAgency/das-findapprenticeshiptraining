using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class CheckboxListFilterSectionViewModel : FilterSection
{
    public List<FilterItemViewModel> Items { get; set; } = [];

    public CheckboxListFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.CheckboxList;
    }
}
