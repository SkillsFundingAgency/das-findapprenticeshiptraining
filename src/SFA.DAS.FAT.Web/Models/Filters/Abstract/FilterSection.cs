using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.Abstract;

public abstract class FilterSection
{
    public required string Id { get; set; }
    public required string For { get; set; }
    public string? Heading { get; set; } = string.Empty;
    public string? SubHeading { get; set; } = string.Empty;
    public SectionLink? Link { get; set; }
    public FilterComponentType FilterComponentType { get; set; }
    public List<FilterSection> Children { get; set; } = [];
}
