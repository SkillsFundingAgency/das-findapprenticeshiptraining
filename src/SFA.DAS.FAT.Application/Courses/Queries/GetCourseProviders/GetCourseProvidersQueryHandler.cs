using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders
{
    public class GetCourseProvidersQueryHandler : IRequestHandler<GetCourseProvidersQuery, GetCourseProvidersResult>
    {
        private readonly ICourseService _courseService;

        public GetCourseProvidersQueryHandler(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<GetCourseProvidersResult> Handle(GetCourseProvidersQuery request, CancellationToken cancellationToken)
        {
            var courseProviders = await _courseService.GetCourseProviders(request.CourseId, request.Location, request.DeliveryModes, request.EmployerProviderRatings, request.ApprenticeProviderRatings, request.Lat, request.Lon, request.ShortlistUserId);

            return new GetCourseProvidersResult
            {
                Course = courseProviders.Course,
                Providers = courseProviders.CourseProviders,
                Total = courseProviders.Total,
                TotalFiltered = courseProviders.TotalFiltered,
                Location = courseProviders.Location?.Name,
                LocationGeoPoint = courseProviders.Location?.LocationPoint?.GeoPoint,
                ShortlistItemCount = courseProviders.ShortlistItemCount,
                ShowEmployerDemand = courseProviders.ShowEmployerDemand
            };
        }
    }
}
