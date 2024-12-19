using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse
{
    public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, GetCourseResult>
    {
        private readonly ICourseService _courseService;

        public GetCourseQueryHandler(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<GetCourseResult> Handle(GetCourseQuery query, CancellationToken cancellationToken)
        {
            var response = await _courseService.GetCourse(query.CourseId, query.Lat, query.Lon, query.LocationName, query.ShortlistUserId);

            return new GetCourseResult
            {
                Course = response?.Course,
                ProvidersCount = response?.ProvidersCount,
                ShortlistItemCount = response?.ShortlistItemCount ?? 0
            };
        }
    }
}
