using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;

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
        var courseProvidersParameters = new CourseProvidersParameters
        {
            LarsCode = request.LarsCode,
            OrderBy = request.OrderBy ?? ProviderOrderBy.Distance,
            Distance = request.Distance,
            Location = request.Location,
            DeliveryModeTypes = request.DeliveryModes,
            EmployerProviderRatingTypes = request.EmployerProviderRatings,
            ApprenticeProviderRatingTypes = request.ApprenticeProviderRatings,
            QarRatings = request.Qar,
            Page = request.Page,
            ShortlistUserId = request.ShortlistUserId
        };

        var courseProvidersDetails = await _courseService.GetCourseProviders(courseProvidersParameters);

        if (courseProvidersDetails == null)
            return null;

        var academicYearsLatest = await _academicYearsService.GetAcademicYearsLatestAsync(cancellationToken);

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
