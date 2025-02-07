using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Filters;

public sealed class ClearFilterSection
{
    public string Title { get; set; }

    public List<ClearFilterItem> Items { get; set; } = [];
}
