using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface ICourseService
    {
        Task<TrainingCourse> GetCourse(int courseId, double lat, double lon, string locationName, Guid? shortlistUserId);
        Task<TrainingCourses> GetCourses(string keyword, List<string> requestRouteIds, List<int> requestLevels, OrderBy orderBy, Guid? shortlistUserId);
        Task<CourseProvidersDetails> GetCourseProviders(int courseId, ProviderOrderBy orderBy, int? distance, string? queryLocation,
            IEnumerable<ProviderDeliveryMode> queryDeliveryModes, IEnumerable<ProviderRating> queryEmployerProviderRatings,
            IEnumerable<ProviderRating> queryApprenticeProviderRatings, IEnumerable<QarRating> qarRatings, int? page, int? pageSize, Guid? shortlistUserId);
        Task<TrainingCourseProviderDetails> GetCourseProviderDetails(int providerId, int standardId,
            string location, double lat, double lon, Guid shortlistUserId);
    }

    public interface IAcademicYearsService
    {
        Task<AcademicYearsLatest> GetAcademicYearsLatest();
    }
}
