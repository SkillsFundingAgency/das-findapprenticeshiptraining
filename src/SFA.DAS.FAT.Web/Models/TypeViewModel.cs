using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class TypeViewModel
{
    public TypeViewModel(ApprenticeType type, ICollection<string> selectedTypes)
    {
        Selected = selectedTypes?.Contains(type.Name) ?? false;
        Code = type.Code;
        Name = type.Name;
    }

    public bool Selected { get; }
    public string Name { get; }
    public string Code { get; }
}
