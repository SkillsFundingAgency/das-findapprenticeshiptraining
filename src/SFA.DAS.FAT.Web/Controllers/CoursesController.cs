using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
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
    private readonly IValidator<GetCourseProviderDetailsQuery> _courseProviderDetailsValidator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ISessionService _sessionService;

    public CoursesController(
        IMediator mediator,
        IOptions<FindApprenticeshipTrainingWeb> config,
        IValidator<GetCourseQuery> courseValidator,
        IValidator<GetCourseProviderDetailsQuery> courseProviderDetailsValidator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        ISessionService sessionService
    )
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _courseValidator = courseValidator;
        _config = config.Value;
        _courseProviderDetailsValidator = courseProviderDetailsValidator;
        _sessionService = sessionService;
    }

    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        int validatedDistance = DistanceService.GetValidDistance(model.Distance, model.Location);

        if(string.IsNullOrWhiteSpace(model.Distance) || !DistanceService.IsValidDistance(model.Distance))
        {
            model.Distance = DistanceService.TEN_MILES.ToString();
        }

        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = model.Location,
            Distance = validatedDistance,
            Routes = model.Categories,
            Levels = model.Levels,
            Page = model.PageNumber,
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
    public async Task<IActionResult> CourseDetails([FromRoute] int id, [FromQuery] string location, [FromQuery] string distance)
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

        var query = new GetCourseQuery
        {
            LarsCode = id,
            Location = location,
            Distance = convertedDistance
        };

        var validationResult = await _courseValidator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors[0].ErrorMessage);
        }

        GetCourseQueryResult result = await _mediator.Send(query);

        if (result == null)
        {
            return RedirectToRoute(RouteNames.Error404);
        }

        var viewModel = (CourseViewModelv2)result;
        viewModel.Location = location;
        viewModel.Distance = distance;

        return View(viewModel);
    }

    [Route("{id:int}/providers/{ProviderId:long}", Name = RouteNames.CourseProviderDetails)]
    public async Task<IActionResult> CourseProviderDetails(int id, int ProviderId, string location, string distance)
    {
        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>();

        if(!string.IsNullOrWhiteSpace(location) && !DistanceService.IsValidDistance(distance))
        {
            distance = DistanceService.TEN_MILES.ToString();
        }
        
        var query = new GetCourseProviderDetailsQuery
        {
            Ukprn = ProviderId,
            LarsCode = id,
            Location = location,
            Distance = string.IsNullOrWhiteSpace(location) ? null : DistanceService.DEFAULT_DISTANCE,
            ShortlistUserId = shortlistUserId
        };

        var validationResult = await _courseProviderDetailsValidator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors[0].ErrorMessage);
        }

        GetCourseProviderQueryResult result = await _mediator.Send(query);

        if (result is null)
        {
            return RedirectToRoute(RouteNames.Error404);
        }

        var viewModel = (CourseProviderViewModel)result;
        viewModel.CourseId = id;
        viewModel.Location = location;
        viewModel.Distance = distance ?? DistanceService.TEN_MILES.ToString();
        viewModel.ShortlistId = result.ShortlistId;
        viewModel.ShowApprenticeTrainingCourseProvidersCrumb = true;
        viewModel.ShowApprenticeTrainingCourseCrumb = true;
        viewModel.ShowApprenticeTrainingCoursesCrumb = true;
        viewModel.ShowShortListLink = true;
        viewModel.ShowSearchCrumb = true;
        viewModel.ShortlistCount = shortlistCount?.Count ?? 0;
        return View(viewModel);
    }
}
