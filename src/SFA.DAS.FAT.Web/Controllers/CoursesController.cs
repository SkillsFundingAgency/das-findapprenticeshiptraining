using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
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
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly FindApprenticeshipTrainingWeb _config;
    private readonly IValidator<GetCourseQuery> _courseValidator;
    private readonly IValidator<GetCourseProviderDetailsQuery> _courseProviderDetailsValidator;
namespace SFA.DAS.FAT.Web.Controllers;

[Route("[controller]")]
public class CoursesController : Controller
{
    private readonly ILogger<CoursesController> _logger;
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieStorageService;
    private readonly ICookieStorageService<GetCourseProvidersRequest> _courseProvidersCookieStorageService;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly FindApprenticeshipTrainingWeb _config;
    private readonly IDataProtector _shortlistDataProtector;
    private readonly IValidator<GetCourseQuery> _courseValidator;
    private readonly IValidator<GetCourseProviderQuery> _courseProviderValidator;

    public CoursesController(
        ILogger<CoursesController> logger,
        IMediator mediator,
        ICookieStorageService<LocationCookieItem> locationCookieStorageService,
        ICookieStorageService<GetCourseProvidersRequest> courseProvidersCookieStorageService,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IDataProtectionProvider provider,
        IOptions<FindApprenticeshipTrainingWeb> config,
        IValidator<GetCourseQuery> courseValidator,
        IValidator<GetCourseProviderDetailsQuery> courseProviderDetailsValidator
    )
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _courseValidator = courseValidator;
        _courseProviderDetailsValidator = courseProviderDetailsValidator;
        _config = config.Value;
    }
    public CoursesController(
        ILogger<CoursesController> logger,
        IMediator mediator,
        ICookieStorageService<LocationCookieItem> locationCookieStorageService,
        ICookieStorageService<GetCourseProvidersRequest> courseProvidersCookieStorageService,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IDataProtectionProvider provider,
        IOptions<FindApprenticeshipTrainingWeb> config,
        IValidator<GetCourseQuery> courseValidator,
        IValidator<GetCourseProviderQuery> courseProviderValidator
    )
    {
        _logger = logger;
        _mediator = mediator;
        _locationCookieStorageService = locationCookieStorageService;
        _courseProvidersCookieStorageService = courseProvidersCookieStorageService;
        _shortlistCookieService = shortlistCookieService;
        _courseValidator = courseValidator;
        _courseProviderValidator = courseProviderValidator;
        _config = config.Value;
        _shortlistDataProtector = provider.CreateProtector(Constants.ShortlistProtectorName);
    }

    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var shortlistCookieItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
    [Route("", Name = RouteNames.Courses)]
    public async Task<IActionResult> Courses(GetCoursesViewModel model)
    {
        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        int validatedDistance = DistanceService.GetValidDistance(model.Distance, model.Location);

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
        var result = await _mediator.Send(new GetCoursesQuery
        {
            Keyword = model.Keyword,
            Location = model.Location,
            Distance = validatedDistance,
            Routes = model.Categories,
            Levels = model.Levels,
            Page = model.PageNumber,
            OrderBy = string.IsNullOrWhiteSpace(model.Keyword) ? OrderBy.Title : OrderBy.Score,
            ShortlistUserId = shortlistItem?.ShortlistUserId
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

        int? convertedDistance = null;

        if(!string.IsNullOrWhiteSpace(location))
        {
            if (distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
            {
                convertedDistance = null;
            }
            else if (!DistanceService.IsValidDistance(distance))
            {
                convertedDistance = DistanceService.TEN_MILES;
                distance = DistanceService.TEN_MILES.ToString();
            }
            else
            {
                convertedDistance = DistanceService.GetValidDistance(distance);
                distance = convertedDistance.ToString();
            }
        }
        
        var query = new GetCourseProviderDetailsQuery
        {
            Ukprn = ProviderId,
            LarsCode = id,
            Location = location,
            Distance = convertedDistance,
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
        
        return View(viewModel);
    }
}
