using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
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
    private readonly IValidator<GetCourseQuery> _courseValidator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;

    public CoursesController(
        IMediator mediator,
        IOptions<FindApprenticeshipTrainingWeb> config,
        IValidator<GetCourseQuery> courseValidator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _courseValidator = courseValidator;
        _config = config.Value;
    }

    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        int validatedDistance = DistanceService.GetValidDistance(model.Distance, model.Location);

        if (string.IsNullOrWhiteSpace(model.Distance) || !DistanceService.IsValidDistance(model.Distance))
        {
            model.Distance = DistanceService.TEN_MILES.ToString();
        }

        var types = new List<ApprenticeType>
        {
            new()
            {
                Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
            },
            new()
            {
                Code = ApprenticeshipType.Apprenticeship.ToString(),
                Name= ApprenticeshipType.Apprenticeship.GetDescription()
            }
        };

        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = model.Location,
            Distance = validatedDistance,
            Routes = model.Categories,
            Levels = model.Levels,
            Page = model.PageNumber,
            ApprenticeshipTypes = model.ApprenticeshipTypes,
            OrderBy = string.IsNullOrWhiteSpace(model.Keyword) ? OrderBy.Title : OrderBy.Score,
            ShortlistUserId = shortlistCookieItem?.ShortlistUserId
        });

        var viewModel = new CoursesViewModel(_config, Url)
        {
            Standards = result.Standards.Select(c => (StandardViewModel)c).ToList(),
            Routes = result.Routes.Select(route => new RouteViewModel(route, model.Categories)).ToList(),
            Total = result.TotalCount,
            TotalFiltered = result.TotalCount,
            Keyword = model.Keyword,
            SelectedRoutes = model.Categories,
            SelectedLevels = model.Levels,
            Levels = result.Levels.Select(level => new LevelViewModel(level, model.Levels)).ToList(),
            Location = model.Location ?? string.Empty,
            Distance = DistanceService.GetDistanceQueryString(model.Distance, model.Location),
            Types = types.Select(type => new TypeViewModel(type, model.ApprenticeshipTypes)).ToList(),
            SelectedTypes = model.ApprenticeshipTypes,
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

    [Route("{id}", Name = RouteNames.CourseDetails)]
    public async Task<IActionResult> CourseDetails([FromRoute] string id, [FromQuery] string location, [FromQuery] string distance)
    {
        int? convertedDistance = null;
        if (distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
        {
            convertedDistance = DistanceService.DEFAULT_DISTANCE;
        }
        else if (!DistanceService.IsValidDistance(distance))
        {
            convertedDistance = DistanceService.TEN_MILES;
            distance = DistanceService.TEN_MILES.ToString();
        }
        else
        {
            convertedDistance = DistanceService.GetValidDistance(distance);
        }

        var query = new GetCourseQuery()
        {
            LarsCode = id.ToString(),
            Location = location,
            Distance = convertedDistance
        };

        var validationResult = await _courseValidator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return NotFound();
        }

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
