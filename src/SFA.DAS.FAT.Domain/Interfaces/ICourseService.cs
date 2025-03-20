using System;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface ICourseService
{
    Task<GetCourseResponse> GetCourse(int larsCode, string location, int? distance);
    Task<CourseProvidersDetails> GetCourseProviders(CourseProvidersParameters courseProvidersParameters);
    Task<TrainingCourseProviderDetails> GetCourseProviderDetails(int providerId, int standardId, string location, double lat, double lon, Guid shortlistUserId);
}
