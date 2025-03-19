using System;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface ICourseService
{
    Task<TrainingCourse> GetCourse(int courseId, double lat, double lon, string locationName, Guid? shortlistUserId);

    Task<CourseProvidersDetails> GetCourseProviders(CourseProvidersParameters courseProvidersParameters);

    Task<TrainingCourseProviderDetails> GetCourseProviderDetails(int providerId, int standardId,
    string location, double lat, double lon, Guid shortlistUserId);
}
