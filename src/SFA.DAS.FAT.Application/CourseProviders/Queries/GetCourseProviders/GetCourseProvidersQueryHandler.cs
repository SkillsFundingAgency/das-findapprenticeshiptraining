using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.CourseProviders.Queries.GetCourseProviders
{
    public class GetCourseProvidersQueryHandler : IRequestHandler<GetCourseProvidersQuery, GetCourseProvidersResult>
    {
        private readonly ICourseService _courseService;
        private readonly IAcademicYearsService _academicYearsService;
        public GetCourseProvidersQueryHandler(ICourseService courseService, IAcademicYearsService academicYearsService)
        {
            _courseService = courseService;
            _academicYearsService = academicYearsService;
        }

        public async Task<GetCourseProvidersResult> Handle(GetCourseProvidersQuery request,
            CancellationToken cancellationToken)
        {
            var courseProvidersDetails = await _courseService.GetCourseProviders(request.Id, request.OrderBy ?? ProviderOrderBy.Distance, request.Distance,
                request.Location,
                request.DeliveryModes, request.EmployerProviderRatings, request.ApprenticeProviderRatings, request.Qar,
                request.Page, request.PageSize, request.ShortlistUserId);

            var academicYearsLatest = await _academicYearsService.GetAcademicYearsLatest();

            return new GetCourseProvidersResult
            {
                LarsCode = courseProvidersDetails.LarsCode,
                Page = courseProvidersDetails.Page,
                PageSize = courseProvidersDetails.PageSize,
                TotalPages = courseProvidersDetails.TotalPages,
                TotalCount = courseProvidersDetails.TotalCount,
                StandardName = courseProvidersDetails.StandardName,
                QarPeriod = academicYearsLatest.QarPeriod,
                ReviewPeriod = academicYearsLatest.ReviewPeriod,
                Providers = courseProvidersDetails.Providers
            };
        }
    }
}
