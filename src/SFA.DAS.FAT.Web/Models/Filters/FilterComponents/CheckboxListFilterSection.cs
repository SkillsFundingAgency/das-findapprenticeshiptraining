using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class CheckboxListFilterSection : FilterSection
{
    public List<FilterItem> Items { get; set; } = new List<FilterItem>();

    public CheckboxListFilterSection()
    {
        FilterComponentType = FilterComponentType.CheckboxList;
    }
}
