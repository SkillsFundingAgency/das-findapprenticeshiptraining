using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class AccordionFilterSectionViewModel : FilterSection
{
    public AccordionFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.Accordion;
    }
}
