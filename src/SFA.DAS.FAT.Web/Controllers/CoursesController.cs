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
    public IActionResult CoursesPost(GetCoursesViewModel postModel)
    {
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = postModel.Location, Distance = postModel.Distance });
        var getModel = new GetCoursesViewModel
        {
            Keyword = postModel.Keyword,
            Categories = postModel.Categories,
            Levels = postModel.Levels,
            LearningTypes = postModel.LearningTypes,
            PageNumber = 1
        };
        return RedirectToRoute(RouteNames.Courses, getModel);
    }

    [HttpGet]
    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        if (Request.Query.ContainsKey("location"))
        {
            _locationCookieService.Delete(Constants.LocationCookieName);
        }

        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var locationCookieItem = _locationCookieService.Get(Constants.LocationCookieName);

        int validatedDistance = DistanceService.GetValidDistance(locationCookieItem?.Distance, locationCookieItem?.Location);
        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = locationCookieItem?.Location,
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
            Standards = [.. result.Standards.Select(c => new StandardViewModel(c, locationCookieItem?.Location, locationCookieItem?.Distance, _config, Url, levels))],
            Routes = [.. result.Routes.Select(route => new RouteViewModel(route, model.Categories))],
            Total = result.TotalCount,
            TotalFiltered = result.TotalCount,
            Keyword = model.Keyword,
            SelectedRoutes = model.Categories,
            SelectedLevels = model.Levels,
            Levels = levels,
            Location = locationCookieItem?.Location ?? string.Empty,
            Distance = DistanceService.GetDistance(locationCookieItem?.Distance, locationCookieItem?.Location),
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
