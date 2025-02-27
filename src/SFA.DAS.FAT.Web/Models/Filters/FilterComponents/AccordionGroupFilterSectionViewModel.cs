using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class AccordionGroupFilterSectionViewModel : FilterSection
{
    public AccordionGroupFilterSectionViewModel()
    {
        FilterComponentType = FilterService.FilterComponentType.AccordionGroup;
    }
}
