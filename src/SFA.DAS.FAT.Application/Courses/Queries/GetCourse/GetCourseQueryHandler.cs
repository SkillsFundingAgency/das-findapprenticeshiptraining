using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Validation;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse
{
    public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, GetCourseResult>
    {
        private readonly ICourseService _courseService;
        private readonly IValidator<GetCourseQuery> _validator;

        public GetCourseQueryHandler(ICourseService courseService, IValidator<GetCourseQuery> validator)
        {
            _courseService = courseService;
            _validator = validator;
        }
        public async Task<GetCourseResult> Handle(GetCourseQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(query);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult,null, null);
            }

            var response = await _courseService.GetCourse(query.CourseId, query.Lat, query.Lon,query.LocationName, query.ShortlistUserId);
            
            return new GetCourseResult
            {
                Course = response?.Course, 
                ProvidersCount = response?.ProvidersCount, 
                ShortlistItemCount = response?.ShortlistItemCount ?? 0
            };
        }
    }
}
