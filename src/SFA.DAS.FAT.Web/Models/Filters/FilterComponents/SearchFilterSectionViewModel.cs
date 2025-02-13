using SFA.DAS.FAT.Web.Models.Filters.Abstract;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class SearchFilterSectionViewModel : FilterSection
{
    public string? InputValue { get; set; }

    public SearchFilterSectionViewModel()
    {
        FilterComponentType = FilterFactory.FilterComponentType.Search;
    }
}
