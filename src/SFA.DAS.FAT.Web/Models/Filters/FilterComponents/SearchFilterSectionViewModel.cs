using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class SearchFilterSectionViewModel : FilterSection
{
    public string InputValue { get; set; }

    public SearchFilterSectionViewModel()
    {
        FilterComponentType = FilterService.FilterComponentType.Search;
    }
}
