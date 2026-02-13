using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.FAT.Web.Models;

public class GetCoursesViewModel
{
    [FromQuery]
    public string Keyword { get; set; }

    [FromQuery]
    public List<string> Categories { get; set; } = [];

    [FromQuery]
    public List<int> Levels { get; set; } = [];

    [FromQuery]
    public List<string> TrainingTypes { get; set; } = [];

    [FromQuery]
    public int PageNumber { get; set; } = 1;

    [FromQuery]
    public string Location { get; set; }

    [FromQuery]
    public string Distance { get; set; }
}
