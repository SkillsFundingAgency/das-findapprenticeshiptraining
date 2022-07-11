using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface ICourseService
    {
        Task<TrainingCourse> GetCourse(int courseId, double lat, double lon, string locationName, Guid? shortlistUserId);
        Task<TrainingCourses> GetCourses(string keyword, List<string> requestRouteIds, List<int> requestLevels, OrderBy orderBy, Guid? shortlistUserId);
        Task<TrainingCourseProviders> GetCourseProviders(     int courseId, string queryLocation,
            IEnumerable<DeliveryModeType> queryDeliveryModes, IEnumerable<ProviderRating> queryEmployerProviderRatings,
            IEnumerable<ProviderRating> queryApprenticeProviderRatings, double lat, double lon, Guid? shortlistUserId);
        Task<TrainingCourseProviderDetails> GetCourseProviderDetails(    int providerId, int standardId,
            string location, double lat, double lon, Guid shortlistUserId);
    }
}
