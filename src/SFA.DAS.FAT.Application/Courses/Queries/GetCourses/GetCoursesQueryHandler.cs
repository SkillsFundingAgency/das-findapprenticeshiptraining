﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler(
    ILevelsService _levelsService,
    IRoutesService _routesService,
    IOptions<FindApprenticeshipTrainingApi> _config,
    IApiClient _apiClient
) : IRequestHandler<GetCoursesQuery, GetCoursesQueryResult>
{
    public async Task<GetCoursesQueryResult> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var levels = await _levelsService.GetLevelsAsync(cancellationToken);

        var routes = await _routesService.GetRoutesAsync(cancellationToken);

        var routeIds = routes.Where(a => query.Routes.Contains(a.Name)).Select(t => t.Id).ToList();

        var apprenticeshipType = string.Empty;

        if (query.ApprenticeshipTypes.Count == 1)
        {
            var type = query.ApprenticeshipTypes.First();
            apprenticeshipType = ApprenticeshipType.Apprenticeship.ToString();

            if (type == ApprenticeshipType.FoundationApprenticeship.GetDescription())
            {
                apprenticeshipType = ApprenticeshipType.FoundationApprenticeship.ToString();
            }
        }

        var coursesResponse = await _apiClient.Get<GetCoursesResponse>(
            new GetCoursesApiRequest{
                BaseUrl = _config.Value.BaseUrl,
                Keyword = query.Keyword,
                Location = query.Location,
                Distance = query.Distance,
                RouteIds = routeIds,
                ApprenticeshipType = apprenticeshipType,
                Levels = query.Levels,
                Page = query.Page,
                OrderBy = query.OrderBy
            }
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
