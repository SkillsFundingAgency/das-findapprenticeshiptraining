using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class TextBoxFilterSection : FilterSection
{
    public string? InputValue { get; set; }

    public TextBoxFilterSection()
    {
        FilterComponentType = FilterComponentType.TextBox;
    }
}
