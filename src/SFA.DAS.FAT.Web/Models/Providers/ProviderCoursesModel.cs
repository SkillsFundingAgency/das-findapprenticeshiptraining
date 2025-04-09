using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; }
    public int CourseCount { get; set; }

    public string CoursesDropdownText { get; set; } = string.Empty;

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        if (source == null) return new ProviderCoursesModel();

        var details = new ProviderCoursesModel();
        var courses = new List<ProviderCourseDetails>();
        foreach (var course in source)
        {
            courses.Add(course);
        }

        details.Courses = courses;
        var coursesCount = courses.Count;
        details.CourseCount = coursesCount;

        details.CoursesDropdownText = coursesCount == 1
            ? $"View 1 course delivered by this training provider"
            : $"View {coursesCount} courses delivered by this training provider";

        return details;
    }
}
