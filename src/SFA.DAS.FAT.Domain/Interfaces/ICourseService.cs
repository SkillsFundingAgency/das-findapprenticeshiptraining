using System;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface ICourseService
{
    Task<GetCourseResponse> GetCourse(string larsCode, string location, int? distance);
    Task<CourseProvidersDetails> GetCourseProviders(CourseProvidersParameters courseProvidersParameters);
    Task<CourseProviderDetailsModel> GetCourseProvider(int ukprn, string larsCode, string location, int? distance, Guid shortlistUserId);
}
