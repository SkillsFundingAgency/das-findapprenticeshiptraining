using System.Collections.Generic;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class ClearFilterSection
{
    public required FilterType FilterType { get; set; }
    public required string Title { get; set; }
    public List<ClearFilterItem> Items { get; set; } = [];
}
