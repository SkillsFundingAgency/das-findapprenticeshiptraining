using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetProvider
{
    public class GetCourseProviderQueryHandler : IRequestHandler<GetCourseProviderQuery, GetCourseProviderResult>
    {
        private readonly ICourseService _courseService;
        public GetCourseProviderQueryHandler(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<GetCourseProviderResult> Handle(GetCourseProviderQuery query, CancellationToken cancellationToken)
        {
            var response = await _courseService.GetCourseProviderDetails(query.ProviderId, query.CourseId, query.Location, query.Lat, query.Lon, query.ShortlistUserId ?? Guid.Empty);

            return new GetCourseProviderResult
            {
                Provider = response?.CourseProviderDetails,
                Course = response?.TrainingCourse,
                AdditionalCourses = response?.AdditionalCourses,
                Location = response?.Location?.Name,
                LocationGeoPoint = response?.Location?.LocationPoint?.GeoPoint,
                ProvidersAtLocation = response?.ProvidersCount?.ProvidersAtLocation ?? 0,
                TotalProviders = response?.ProvidersCount?.TotalProviders ?? 0,
                ShortlistItemCount = response?.ShortlistItemCount ?? 0
            };
        }
    }
}
