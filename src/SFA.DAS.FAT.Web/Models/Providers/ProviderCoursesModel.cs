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

    public int CourseCount => Courses.Count;

    public string CoursesDropdownText
    {
        get
        {
            if (Courses.Count == 0)
                return string.Empty;

            return CourseCount == 1
                ? "View 1 course delivered by this training provider"
                : $"View {CourseCount} courses delivered by this training provider";
        }
    }

    public static readonly LearningType[] LearningTypeOrder =
        new[] { LearningType.ApprenticeshipUnit, LearningType.FoundationApprenticeship, LearningType.Apprenticeship };

    private static readonly Dictionary<LearningType, string> LearningTypeDisplayNames = new Dictionary<LearningType, string>
    {
        [LearningType.ApprenticeshipUnit] = "Apprenticeship unit",
        [LearningType.FoundationApprenticeship] = "Foundation apprenticeship",
        [LearningType.Apprenticeship] = "Apprenticeship"
    };

    private static string GetLearningTypeDisplayName(LearningType learningType) =>
        LearningTypeDisplayNames.TryGetValue(learningType, out var displayName)
            ? displayName
            : "Apprenticeship";

    public IList<CourseGroupViewModel> GetCourseGroups()
    {
        if (Courses.Count == 0)
        {
            return new List<CourseGroupViewModel>();
        }

        var groupedCourses = Courses
            .GroupBy(c => c.ApprenticeshipType)
            .ToDictionary(group => group.Key, group => group.ToList());

        var courseGroups = new List<CourseGroupViewModel>();

        foreach (var learningType in LearningTypeOrder)
        {
            if (!groupedCourses.TryGetValue(learningType, out var coursesByType) || coursesByType.Count == 0)
            {
                continue;
            }

            courseGroups.Add(CreateCourseGroup(learningType, coursesByType));
        }

        return courseGroups;
    }

    private CourseGroupViewModel CreateCourseGroup(
        LearningType learningType,
        List<ProviderCourseDetails> coursesByType)
    {
        var learningTypeDisplayName = GetLearningTypeDisplayName(learningType);
        var pluralDisplayName = learningTypeDisplayName.Pluralize();
        return new CourseGroupViewModel
        {
            Ukprn = Ukprn,
            Location = Location,
            ApprenticeshipType = learningType,
            DisplayNameHeader = pluralDisplayName,
            DisplayName = coursesByType.Count > 1 ? pluralDisplayName : learningTypeDisplayName,
            Courses = coursesByType,
            Count = coursesByType.Count
        };
    }

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList() ?? new List<ProviderCourseDetails>()
        };

        return model;
    }

    public static implicit operator ProviderCoursesModel(List<ProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => c).ToList() ?? new List<ProviderCourseDetails>()
        };

        return model;
    }
}
