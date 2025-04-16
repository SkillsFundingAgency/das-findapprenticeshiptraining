using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

public class GetCourseProviderQueryHandler : IRequestHandler<GetCourseProviderDetailsQuery, GetCourseProviderQueryResult>
{
    private readonly ICourseService _courseService;

    public GetCourseProviderQueryHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<GetCourseProviderQueryResult> Handle(GetCourseProviderDetailsQuery query, CancellationToken cancellationToken)
    {
        return await _courseService.GetCourseProvider(
            query.Ukprn, 
            query.LarsCode, 
            query.Location, 
            query.Distance,
            query.ShortlistUserId ?? Guid.Empty
        );
    }
}
