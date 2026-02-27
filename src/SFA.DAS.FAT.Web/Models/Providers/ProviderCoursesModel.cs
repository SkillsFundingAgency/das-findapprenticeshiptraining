using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; } = new List<ProviderCourseDetails>();
    public int CourseCount { get; set; }

    public string CoursesDropdownText { get; set; } = string.Empty;

    public ApprenticeshipType[] ApprenticeshipTypeOrder => new[] { ApprenticeshipType.ApprenticeshipUnit, ApprenticeshipType.FoundationApprenticeship, ApprenticeshipType.Apprenticeship };

    public static string GetDisplayName(ApprenticeshipType apprenticeshipType)
    {
        return apprenticeshipType switch
        {
            ApprenticeshipType.ApprenticeshipUnit => "Apprenticeship units",
            ApprenticeshipType.FoundationApprenticeship => "Foundation apprenticeships",
            _ => "Apprenticeships"
        };
    }

    public List<ProviderCourseGroup> CourseGroups => ApprenticeshipTypeOrder
        .Select(apprenticeshipType => new ProviderCourseGroup
        {
            Type = apprenticeshipType,
            DisplayName = GetDisplayName(apprenticeshipType),
            Courses = Courses?.Where(c => c.ApprenticeshipType == apprenticeshipType).ToList() ?? new List<ProviderCourseDetails>(),
            Count = Courses?.Count(c => c.ApprenticeshipType == apprenticeshipType) ?? 0
        })
        .Where(g => g.Count > 0)
        .ToList();

    public void SetRouteDataForCourses(int ukprn)
    {
        if (Courses == null) return;

        foreach (var course in Courses)
        {
            course.RouteData = new Dictionary<string, string>
                {
                    { "larsCode", course.LarsCode?.ToString() ?? string.Empty },
                    { "providerId", ukprn.ToString() }
                };
        }
    }

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
