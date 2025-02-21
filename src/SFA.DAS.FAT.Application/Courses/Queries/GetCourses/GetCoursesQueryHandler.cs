using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler(
    ICourseService _courseService, 
    ILevelsService _levelsService,
    IRoutesService _routesService
) : IRequestHandler<GetCoursesQuery, GetCoursesQueryResult>
{
    public async Task<GetCoursesQueryResult> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var levels = await _levelsService.GetLevelsAsync(cancellationToken);

        var routes = await _routesService.GetRoutesAsync(cancellationToken);

        var routeIds = routes.Where(a => query.Routes.Contains(a.Name)).Select(t => t.Id).ToList();

        var coursesResponse = await _courseService.GetCourses(
            query.Keyword,
            query.Location,
            query.Distance,
            routeIds,
            query.Levels, 
            query.Page,
            query.OrderBy,
            cancellationToken
        );

        return new GetCoursesQueryResult()
        {
            Standards = coursesResponse.Standards,
            Page = coursesResponse.Page,
            PageSize = coursesResponse.PageSize,
            TotalPages = coursesResponse.TotalPages,
            TotalCount = coursesResponse.TotalCount,
            Levels = levels,
            Routes = routes
        };
    }
}
