using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; } = new();

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

    public static string GetDisplayNamePlural(ApprenticeshipType apprenticeshipType) =>
        apprenticeshipType switch
        {
            ApprenticeshipType.ApprenticeshipUnit => "Apprenticeship units",
            ApprenticeshipType.FoundationApprenticeship => "Foundation apprenticeships",
            _ => "Apprenticeships"
        };

    public static string GetDisplayNameSingular(ApprenticeshipType apprenticeshipType) =>
       apprenticeshipType switch
       {
           ApprenticeshipType.ApprenticeshipUnit => "Apprenticeship unit",
           ApprenticeshipType.FoundationApprenticeship => "Foundation apprenticeship",
           _ => "Apprenticeship"
       };

    public IList<ProviderCourseGroup> GetCourseGroups()
    {
        if (Courses == null || Courses.Count == 0)
        {
            return new List<ProviderCourseGroup>();
        }

        return ApprenticeshipTypeOrder
            .Select(apprenticeshipType => new ProviderCourseGroup
            {
                ApprenticeshipType = apprenticeshipType,
                DisplayNameHeader = GetDisplayNamePlural(apprenticeshipType),
                DisplayName = Courses.Count(c => c.ApprenticeshipType == apprenticeshipType) > 1 ? GetDisplayNamePlural(apprenticeshipType) : GetDisplayNameSingular(apprenticeshipType),
                Courses = Courses.Where(c => c.ApprenticeshipType == apprenticeshipType).ToList(),
                Count = Courses.Count(c => c.ApprenticeshipType == apprenticeshipType)
            })
            .Where(g => g.Count > 0)
            .ToList();
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
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList()
        };

        return model;
    }
}
