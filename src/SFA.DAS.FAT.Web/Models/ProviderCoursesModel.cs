using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Models;

public class ProviderCoursesModel
{
    public int Ukprn { get; set; }
    public string Location { get; set; }
    public List<ProviderCourseDetails> Courses { get; set; }

    public int CourseCount => Courses?.Count ?? 0;

    public string CoursesDropdownText => ProviderCoursesHelper.CoursesDropdownText(Courses);

    public IList<ProviderCourseGroup> GetCourseGroups()
    {
        return ProviderCoursesHelper.GetCourseGroups(Courses);
    }

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList()
        };

        return model;
    }
    public static implicit operator ProviderCoursesModel(List<ProviderCourseModel> source)
    {
        var model = new ProviderCoursesModel
        {
            Courses = source?.Select(c => (ProviderCourseDetails)c).ToList()
        };

        return model;
    }
}
