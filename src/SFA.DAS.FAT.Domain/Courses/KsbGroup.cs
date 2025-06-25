using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Domain.Courses;

public class KsbGroup

{
    public KsbType Type { get; set; }
    public string Title => Type.GetDescription();
    public List<string> Details { get; set; }
}
