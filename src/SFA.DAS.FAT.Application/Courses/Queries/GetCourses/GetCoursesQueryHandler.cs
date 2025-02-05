using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler(
    ICourseService _courseService, 
    ILevelsService _levelsService,
    IRoutesService _routesService
) : IRequestHandler<GetCoursesQuery, GetCoursesResult>
{
    public async Task<GetCoursesResult> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var response = await _courseService.GetCourses(query.Keyword, query.RouteIds, query.Levels, query.OrderBy, query.ShortlistUserId);

        var levels = await _levelsService.GetLevelsAsync(cancellationToken);

        var routes = await _routesService.GetRoutesAsync(cancellationToken);

        return new GetCoursesResult
        {
            Courses = response.Courses.ToList(),
            Routes = routes,
            Total = response.Total,
            TotalFiltered = response.TotalFiltered,
            Levels = levels,
            ShortlistItemCount = response.ShortlistItemCount
        };
    }
}
