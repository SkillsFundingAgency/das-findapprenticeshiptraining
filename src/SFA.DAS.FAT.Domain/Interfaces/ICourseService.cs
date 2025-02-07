using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface ICourseService
{
    Task<TrainingCourse> GetCourse(int courseId, double lat, double lon, string locationName, Guid? shortlistUserId);
    Task<GetCoursesResponse> GetCourses(string keyword, List<int> routeIds, List<int> levels, OrderBy orderBy, CancellationToken cancellationToken);
    Task<TrainingCourseProviders> GetCourseProviders(int courseId, string queryLocation,
    IEnumerable<DeliveryModeType> queryDeliveryModes, IEnumerable<ProviderRating> queryEmployerProviderRatings,
    IEnumerable<ProviderRating> queryApprenticeProviderRatings, double lat, double lon, Guid? shortlistUserId);
    Task<TrainingCourseProviderDetails> GetCourseProviderDetails(    int providerId, int standardId,
    string location, double lat, double lon, Guid shortlistUserId);
}
