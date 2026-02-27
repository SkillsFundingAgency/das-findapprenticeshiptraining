using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; }

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

    public static string GetDisplayName(ApprenticeshipType apprenticeshipType) =>
        apprenticeshipType switch
        {
            ApprenticeshipType.ApprenticeshipUnit => "Apprenticeship units",
            ApprenticeshipType.FoundationApprenticeship => "Foundation apprenticeships",
            _ => "Apprenticeships"
        };

    public IReadOnlyList<ProviderCourseGroup> CourseGroups => ApprenticeshipTypeOrder
        .Select(apprenticeshipType => new ProviderCourseGroup
        {
            ApprenticeshipType = apprenticeshipType,
            DisplayName = GetDisplayName(apprenticeshipType),
            Courses = Courses?.Where(c => c.ApprenticeshipType == apprenticeshipType).ToList() ?? new List<ProviderCourseDetails>(),
            Count = Courses?.Count(c => c.ApprenticeshipType == apprenticeshipType) ?? 0
        })
        .Where(g => g.Count > 0)
        .ToList();

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList()
        };

        return model;
    }
}
