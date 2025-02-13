using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.Models.Filters;

public sealed class FiltersViewModel
{
    public required string Route { get; set; }
    public IReadOnlyList<FilterSection> FilterSections { get; set; } = [];
    public IReadOnlyList<ClearFilterSectionViewModel> ClearFilterSections { get; set; } = [];
    public bool ShowFilterOptions => ClearFilterSections.Count > 0;
}
