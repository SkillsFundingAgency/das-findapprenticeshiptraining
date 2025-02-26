using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class TextBoxFilterSectionViewModel : FilterSection
{
    public string InputValue { get; set; }

    public TextBoxFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.TextBox;
    }
}
