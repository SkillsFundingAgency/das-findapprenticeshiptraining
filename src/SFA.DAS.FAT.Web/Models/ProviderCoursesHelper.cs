using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models;

public static class ProviderCoursesHelper
{
    public static string CoursesDropdownText(IEnumerable<ProviderCourseDetails> courses)
    {
        if (courses == null)
            return string.Empty;

        var count = courses.Count();
        return count == 1
            ? "View 1 course delivered by this training provider"
            : $"View {count} courses delivered by this training provider";
    }

    public static readonly ApprenticeshipType[] ApprenticeshipTypeOrder =
        new[] { ApprenticeshipType.ApprenticeshipUnit, ApprenticeshipType.FoundationApprenticeship, ApprenticeshipType.Apprenticeship };

    public static string GetDisplayName(ApprenticeshipType apprenticeshipType) =>
        apprenticeshipType switch
        {
            ApprenticeshipType.ApprenticeshipUnit => "Apprenticeship units",
            ApprenticeshipType.FoundationApprenticeship => "Foundation apprenticeships",
            _ => "Apprenticeships"
        };

    public static IList<ProviderCourseGroup> GetCourseGroups(IEnumerable<ProviderCourseDetails> courses)
    {
        if (courses == null || !courses.Any())
        {
            return new List<ProviderCourseGroup>();
        }

        return ApprenticeshipTypeOrder
            .Select(apprenticeshipType => new ProviderCourseGroup
            {
                ApprenticeshipType = apprenticeshipType,
                DisplayName = GetDisplayName(apprenticeshipType),
                Courses = courses.Where(c => c.ApprenticeshipType == apprenticeshipType).ToList(),
                Count = courses.Count(c => c.ApprenticeshipType == apprenticeshipType)
            })
            .Where(g => g.Count > 0)
            .ToList();
    }
}
