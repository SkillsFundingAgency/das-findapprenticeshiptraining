using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesSubmitModel
{
    public string Keyword { get; set; }

    public string Location { get; set; }

    public string Distance { get; set; }

    public List<string> Categories { get; set; } = [];

    public List<int> Levels { get; set; } = [];

    public List<LearningType> LearningTypes { get; set; } = [];

    public int PageNumber { get; set; } = 1;

}
