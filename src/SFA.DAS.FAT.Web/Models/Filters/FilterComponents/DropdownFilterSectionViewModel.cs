using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class DropdownFilterSectionViewModel : FilterSection
{
    public List<FilterItemViewModel> Items { get; set; } = new List<FilterItemViewModel>();

    public DropdownFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.Dropdown;
    }
}
