﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface ICourseService
    {
        Task<TrainingCourse> GetCourse(int courseId, double lat, double lon);
        Task<TrainingCourses> GetCourses(string keyword, List<string> requestRouteIds, List<int> requestLevels, OrderBy orderBy);
        Task<TrainingCourseProviders> GetCourseProviders(     int courseId, string queryLocation,
            IEnumerable<DeliveryModeType> queryDeliveryModes, IEnumerable<ProviderRating> queryProviderRatings,
            double lat, double lon, Guid? requestShortlistUserId);
        Task<TrainingCourseProviderDetails> GetCourseProviderDetails(int providerId, int standardId, string location, double lat, double lon);
    }
}
