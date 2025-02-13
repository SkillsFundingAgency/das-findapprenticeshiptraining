using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public class AccordionFilterSection : FilterSection
{
    public AccordionFilterSection()
    {
        FilterComponentType = FilterComponentType.Accordion;
    }
}
