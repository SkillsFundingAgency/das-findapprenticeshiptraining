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
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Extensions;
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
    private readonly IValidator<GetCourseLocationQuery> _courseLocationValidator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;

    public CoursesController(
        IMediator mediator,
        IOptions<FindApprenticeshipTrainingWeb> config,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        ICookieStorageService<LocationCookieItem> locationCookieService,
        IValidator<GetCourseLocationQuery> courseLocationValidator)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _locationCookieService = locationCookieService;
        _config = config.Value;
        _courseLocationValidator = courseLocationValidator;
    }

    [HttpPost]
    [Route("", Name = RouteNames.Courses)]
    public IActionResult CoursesPost(GetCoursesViewModel submitModel)
    {
        var model = new GetCoursesViewModel
        {
            Keyword = submitModel.Keyword,
            Categories = submitModel.Categories,
            Levels = submitModel.Levels,
            LearningTypes = submitModel.LearningTypes,
            PageNumber = 1
        };
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location, Distance = submitModel.Distance });
        return RedirectToRoute(RouteNames.Courses, model);
    }

    [HttpGet]
    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var hasDeletedLocationCookie = false;
        if (Request.Query.ContainsKey(FilterService.ClearLocationQueryParameter))
        {
            _locationCookieService.Delete(Constants.LocationCookieName);
            hasDeletedLocationCookie = true;
        }

        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var locationCookieItem = _locationCookieService.Get(Constants.LocationCookieName);

        if (hasDeletedLocationCookie)
        {
            locationCookieItem = null;
        }
        var requestLocation = locationCookieItem?.Location?.Trim();
        var requestDistance = locationCookieItem?.Distance;

        int validatedDistance = DistanceService.GetValidDistance(requestDistance, requestLocation);
        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = requestLocation,
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
            Standards = [.. result.Standards.Select(c => new StandardViewModel(c, requestLocation, requestDistance, _config, Url, levels))],
            Routes = [.. result.Routes.Select(route => new RouteViewModel(route, model.Categories))],
            Total = result.TotalCount,
            TotalFiltered = result.TotalCount,
            Keyword = model.Keyword,
            SelectedRoutes = model.Categories,
            SelectedLevels = model.Levels,
            Levels = levels,
            Location = requestLocation ?? string.Empty,
            Distance = DistanceService.GetDistance(requestDistance, requestLocation),
            SelectedTrainingTypes = model.LearningTypes,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };
        if (!string.IsNullOrWhiteSpace(requestLocation))
        {
            var validationLocationResult = await _courseLocationValidator.ValidateAsync(new GetCourseLocationQuery { Location = requestLocation });
            if (!validationLocationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationLocationResult.Errors);
            }
        }

        if (result.Standards.Count > 0)
        {
            var paginationQueryParams = viewModel.ToQueryString();
            paginationQueryParams.RemoveAll(q =>
               string.Equals(q.Item1, nameof(CoursesViewModel.Location), System.StringComparison.OrdinalIgnoreCase) ||
               string.Equals(q.Item1, nameof(CoursesViewModel.Distance), System.StringComparison.OrdinalIgnoreCase)
           );
            viewModel.Pagination = new PaginationViewModel(
                result.Page,
                result.TotalCount,
                result.PageSize,
                Url,
                RouteNames.Courses,
                paginationQueryParams
            );
        }

        return View(viewModel);
    }

    [HttpPost]
    [Route("{larsCode}", Name = RouteNames.CourseDetails)]
    public IActionResult CourseDetailsPost(CoursesViewModel model, [FromRoute] string larsCode)
    {
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = model.Location, Distance = model.Distance });
        return RedirectToRoute(RouteNames.CourseDetails, new { larsCode });
    }

    [HttpGet]
    [Route("{larsCode}/remove-location", Name = RouteNames.CourseDetailsRemoveLocation)]
    public IActionResult CourseDetailsDelete([FromRoute] string larsCode)
    {
        _locationCookieService.Delete(Constants.LocationCookieName);
        return RedirectToRoute(RouteNames.CourseDetails, new { larsCode });
    }

    [HttpGet]
    [Route("{larsCode}", Name = RouteNames.CourseDetails)]
    public async Task<IActionResult> CourseDetails([FromRoute] string larsCode)
    {
        var locationCookieItem = _locationCookieService.Get(Constants.LocationCookieName);

        if (string.IsNullOrEmpty(larsCode)) return NotFound();

        var convertedDistance = DistanceService.GetConvertedDistanceForDetails(locationCookieItem?.Distance, locationCookieItem?.Location);

        var query = new GetCourseQuery()
        {
            LarsCode = larsCode,
            Location = locationCookieItem?.Location,
            Distance = convertedDistance
        };

        GetCourseQueryResult result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        var viewModel = (CourseViewModel)result;
        viewModel.Location = locationCookieItem?.Location;
        viewModel.Distance = convertedDistance.ToString();
        viewModel.RequestApprenticeshipTrainingUrl = _config.RequestApprenticeshipTrainingUrl;
        viewModel.EmployerAccountsUrl = _config.EmployerAccountsUrl;

        return View(viewModel);
    }
}
