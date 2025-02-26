using SFA.DAS.FAT.Domain.Courses;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Models
{
    public class LevelViewModel
    {
        public LevelViewModel()
        {

        }

        public LevelViewModel(Level level, ICollection<int> selectedCodes)
        {
            Selected = selectedCodes?.Contains(level.Code) ?? false;
            Code = level.Code;
            Name = level.Name;
        }

        public bool Selected { get; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Title => $"Level {Code} - {Name}";

    }
}
