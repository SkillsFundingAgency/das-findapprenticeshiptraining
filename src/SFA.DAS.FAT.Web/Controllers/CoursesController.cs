using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("[controller]")]
public class CoursesController : Controller
{
    private readonly IMediator _mediator;
    private readonly FindApprenticeshipTrainingWeb _config;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;

    public CoursesController(
        IMediator mediator,
        IOptions<FindApprenticeshipTrainingWeb> config,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _config = config.Value;
    }

    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        int validatedDistance = DistanceService.GetValidDistance(model.Distance, model.Location);

        if (string.IsNullOrWhiteSpace(model.Distance) || !DistanceService.IsValidDistance(model.Distance))
        {
            model.Distance = DistanceService.TenMiles.ToString();
        }

        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = model.Location,
            Distance = validatedDistance,
            Routes = model.Categories,
            Levels = model.Levels,
            Page = model.PageNumber,
            LearningTypes = model.LearningTypes,
            OrderBy = string.IsNullOrWhiteSpace(model.Keyword) ? OrderBy.Title : OrderBy.Score,
            ShortlistUserId = shortlistCookieItem?.ShortlistUserId
        });

        List<LevelViewModel> levels = [.. result.Levels.Select(level => new LevelViewModel(level, model.Levels))];
        var viewModel = new CoursesViewModel()
        {
            Standards = [.. result.Standards.Select(c => new StandardViewModel(c, model.Location, model.Distance, _config, Url, levels))],
            Routes = [.. result.Routes.Select(route => new RouteViewModel(route, model.Categories))],
            Total = result.TotalCount,
            TotalFiltered = result.TotalCount,
            Keyword = model.Keyword,
            SelectedRoutes = model.Categories,
            SelectedLevels = model.Levels,
            Levels = levels,
            Location = model.Location ?? string.Empty,
            Distance = DistanceService.GetDistanceQueryString(model.Distance, model.Location),
            SelectedTrainingTypes = model.LearningTypes,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        if (result.Standards.Count > 0)
        {
            viewModel.Pagination = new PaginationViewModel(
                result.Page,
                result.TotalCount,
                result.PageSize,
                Url,
                RouteNames.Courses,
                viewModel.ToQueryString()
            );
        }

        return View(viewModel);
    }

    [Route("{larsCode}", Name = RouteNames.CourseDetails)]
    public async Task<IActionResult> CourseDetails([FromRoute] string larsCode, [FromQuery] string location, [FromQuery] string distance)
    {
        if (string.IsNullOrEmpty(larsCode)) return NotFound();

        int? convertedDistance = null;
        if (distance == DistanceService.AcrossEnglandFilterValue)
        {
            convertedDistance = DistanceService.DefaultDistance;
        }
        else if (!DistanceService.IsValidDistance(distance))
        {
            convertedDistance = DistanceService.TenMiles;
            distance = DistanceService.TenMiles.ToString();
        }
        else
        {
            convertedDistance = DistanceService.GetValidDistance(distance);
        }

        var query = new GetCourseQuery()
        {
            LarsCode = larsCode,
            Location = location,
            Distance = convertedDistance
        };

        GetCourseQueryResult result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        var viewModel = (CourseViewModel)result;
        viewModel.Location = location;
        viewModel.Distance = distance;

        return View(viewModel);
    }
}
