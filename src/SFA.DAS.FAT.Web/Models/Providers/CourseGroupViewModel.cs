using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class CourseGroupViewModel
{
    public int Ukprn { get; set; }
    public string Location { get; set; }
    public LearningType ApprenticeshipType { get; set; }
    public string DisplayNameHeader { get; set; }
    public string DisplayName { get; set; }
    public int Count { get; set; }
    public List<ProviderCourseDetails> Courses { get; set; } = new();
}
