using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCourseGroup
{
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public string DisplayName { get; set; }
    public int Count { get; set; }
    public List<ProviderCourseDetails> Courses { get; set; } = new();
}
