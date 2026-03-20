using System.Collections.Generic;
using System.Linq;
using Humanizer;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; } = new();

    public int Ukprn { get; set; }
    public string Location { get; set; }

    public int CourseCount => Courses?.Count ?? 0;

    public string CoursesDropdownText
    {
        get
        {
            if (Courses == null)
                return string.Empty;

            return CourseCount == 1
                ? "View 1 course delivered by this training provider"
                : $"View {CourseCount} courses delivered by this training provider";
        }
    }

    public static readonly ApprenticeshipType[] ApprenticeshipTypeOrder =
        new[] { ApprenticeshipType.ApprenticeshipUnit, ApprenticeshipType.FoundationApprenticeship, ApprenticeshipType.Apprenticeship };

    private static readonly Dictionary<ApprenticeshipType, string> ApprenticeshipDisplayNames = new Dictionary<ApprenticeshipType, string>
    {
        [ApprenticeshipType.ApprenticeshipUnit] = "Apprenticeship unit",
        [ApprenticeshipType.FoundationApprenticeship] = "Foundation apprenticeship",
        [ApprenticeshipType.Apprenticeship] = "Apprenticeship"
    };

    private static string GetApprenticeshipTypeDisplayName(ApprenticeshipType apprenticeshipType) =>
        ApprenticeshipDisplayNames.TryGetValue(apprenticeshipType, out var displayName)
            ? displayName
            : "Apprenticeship";

    public IList<CourseGroupViewModel> GetCourseGroups()
    {
        if (Courses == null || Courses.Count == 0)
        {
            return new List<CourseGroupViewModel>();
        }

        var groupedCourses = Courses
            .GroupBy(c => c.ApprenticeshipType)
            .ToDictionary(group => group.Key, group => group.ToList());

        var courseGroups = new List<CourseGroupViewModel>();

        foreach (var apprenticeshipType in ApprenticeshipTypeOrder)
        {
            if (!groupedCourses.TryGetValue(apprenticeshipType, out var coursesByType) || coursesByType.Count == 0)
            {
                continue;
            }

            courseGroups.Add(CreateCourseGroup(apprenticeshipType, coursesByType));
        }

        return courseGroups;
    }

    private CourseGroupViewModel CreateCourseGroup(
        ApprenticeshipType apprenticeshipType,
        List<ProviderCourseDetails> coursesByType)
    {
        var apprenticeshipTypeDisplayName = GetApprenticeshipTypeDisplayName(apprenticeshipType);
        var pluralDisplayName = apprenticeshipTypeDisplayName.Pluralize();

        return new CourseGroupViewModel
        {
            Ukprn = Ukprn,
            Location = Location,
            ApprenticeshipType = apprenticeshipType,
            DisplayNameHeader = pluralDisplayName,
            DisplayName = coursesByType.Count > 1 ? pluralDisplayName : apprenticeshipTypeDisplayName,
            Courses = coursesByType,
            Count = coursesByType.Count
        };
    }

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList()
        };

        return model;
    }

    public static implicit operator ProviderCoursesModel(List<ProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => c).ToList()
        };

        return model;
    }
}
