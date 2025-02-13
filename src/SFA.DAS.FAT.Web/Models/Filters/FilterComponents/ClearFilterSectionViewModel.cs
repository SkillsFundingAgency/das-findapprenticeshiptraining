using System.Collections.Generic;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class ClearFilterSectionViewModel
{
    public required FilterType FilterType { get; set; }
    public required string Title { get; set; }
    public List<ClearFilterItemViewModel> Items { get; set; } = [];
}
