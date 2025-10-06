using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public class GetCourseQueryHandler(ICourseService courseService, ILevelsService levelsService) : IRequestHandler<GetCourseQuery, GetCourseQueryResult>
{
    public async Task<GetCourseQueryResult> Handle(GetCourseQuery query, CancellationToken cancellationToken)
    {
        var levelsResponse = await levelsService.GetLevelsAsync(cancellationToken);

        var courseResponse = await courseService.GetCourse(query.LarsCode, query.Location, query.Distance);

        if (courseResponse == null)
        {
            return null;
        }

        var result = (GetCourseQueryResult)courseResponse;
        result.Levels = levelsResponse.ToList();

        return result;
    }
}
