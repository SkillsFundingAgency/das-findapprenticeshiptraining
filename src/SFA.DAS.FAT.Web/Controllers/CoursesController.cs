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
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;

    public CoursesController(
        IMediator mediator,
        IOptions<FindApprenticeshipTrainingWeb> config,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        ICookieStorageService<LocationCookieItem> locationCookieService)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _locationCookieService = locationCookieService;
        _config = config.Value;
    }

    [HttpPost]
    [Route("", Name = RouteNames.Courses)]
    public IActionResult ApplyFilters([FromForm] CoursesFiltersSubmitModel submitModel)
    {
        var model = new CoursesFiltersRequestModel
        {
            Keyword = submitModel.Keyword,
            Categories = submitModel.Categories,
            Levels = submitModel.Levels,
            LearningTypes = submitModel.LearningTypes,
            PageNumber = 1
        };
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location?.Trim(), Distance = submitModel.Distance });
        return RedirectToRoute(RouteNames.Courses, model);
    }

    [HttpGet]
    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(CoursesFiltersRequestModel requestModel, bool clearFilter = false)
    {
        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        var (requestLocation, requestDistance) = _locationCookieService.GetLocation();
        if (clearFilter)
        {
            _locationCookieService.Delete(Constants.LocationCookieName);
            requestLocation = string.Empty;
            requestDistance = DistanceService.TenMiles.ToString();
        }

        int validatedDistance = DistanceService.GetValidDistance(requestDistance, requestLocation);
        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = requestModel.Keyword,
            Location = requestLocation,
            Distance = validatedDistance,
            Routes = requestModel.Categories,
            Levels = requestModel.Levels,
            Page = requestModel.PageNumber,
            LearningTypes = requestModel.LearningTypes,
            OrderBy = string.IsNullOrWhiteSpace(requestModel.Keyword) ? OrderBy.Title : OrderBy.Score,
            ShortlistUserId = shortlistCookieItem?.ShortlistUserId
        });

        List<LevelViewModel> levels = [.. result.Levels.Select(level => new LevelViewModel(level, requestModel.Levels))];
        var viewModel = new CoursesViewModel()
        {
            Standards = [.. result.Standards.Select(c => new StandardViewModel(c, requestLocation, requestDistance, _config, Url, levels))],
            Routes = [.. result.Routes.Select(route => new RouteViewModel(route, requestModel.Categories))],
            Total = result.TotalCount,
            TotalFiltered = result.TotalCount,
            Keyword = requestModel.Keyword,
            SelectedRoutes = requestModel.Categories,
            SelectedLevels = requestModel.Levels,
            Levels = levels,
            Location = requestLocation ?? string.Empty,
            Distance = DistanceService.GetDistance(requestDistance, requestLocation),
            SelectedTrainingTypes = requestModel.LearningTypes,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        if (result.Standards.Count > 0)
        {
            var paginationQueryParams = viewModel.ToQueryString();
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
    public IActionResult CourseDetailsPost([FromForm] CourseLocationSubmitModel submitModel, [FromRoute] string larsCode)
    {
        var (_, requestDistance) = _locationCookieService.GetLocation();
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location?.Trim(), Distance = requestDistance });
        return RedirectToRoute(RouteNames.CourseDetails, new { larsCode });
    }

    [HttpGet]
    [Route("{larsCode}", Name = RouteNames.CourseDetails)]
    public async Task<IActionResult> CourseDetails([FromRoute] string larsCode, bool clearLocation = false)
    {
        if (string.IsNullOrEmpty(larsCode)) return NotFound();

        var (requestLocation, requestDistance) = _locationCookieService.GetLocation();
        if (clearLocation)
        {
            _locationCookieService?.Delete(Constants.LocationCookieName);
            requestLocation = string.Empty;
            requestDistance = DistanceService.TenMiles.ToString();
        }

        var convertedDistance = DistanceService.GetConvertedDistanceForDetails(requestDistance, requestLocation);

        var query = new GetCourseQuery()
        {
            LarsCode = larsCode,
            Location = requestLocation,
            Distance = convertedDistance
        };

        GetCourseQueryResult result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        var viewModel = (CourseViewModel)result;
        viewModel.Location = requestLocation;
        viewModel.Distance = convertedDistance.ToString();
        viewModel.RequestApprenticeshipTrainingUrl = _config.RequestApprenticeshipTrainingUrl;
        viewModel.EmployerAccountsUrl = _config.EmployerAccountsUrl;

        return View(viewModel);
    }
}
