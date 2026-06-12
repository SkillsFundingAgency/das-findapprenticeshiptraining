using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("courses/{larsCode}/providers")]
public class CourseProvidersController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;
    private readonly IValidator<GetCourseProviderDetailsQuery> _ukprnValidator;
    private readonly IValidator<GetCourseLocationQuery> _courseLocationValidator;
    private readonly IValidator<GetCourseQuery> _courseIdValidator;
    private readonly ISessionService _sessionService;
    private readonly IDateTimeService _dateTimeService;
    private readonly FindApprenticeshipTrainingWeb _config;

    public CourseProvidersController(
        IMediator mediator,
        IValidator<GetCourseProviderDetailsQuery> ukprnValidator,
        IValidator<GetCourseLocationQuery> courseLocationValidator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IOptions<FindApprenticeshipTrainingWeb> config,
        ISessionService sessionService,
        IDateTimeService dateTimeService,
        IValidator<GetCourseQuery> courseIdValidator,
        ICookieStorageService<LocationCookieItem> locationCookieService)
    {
        _mediator = mediator;
        _ukprnValidator = ukprnValidator;
        _courseLocationValidator = courseLocationValidator;
        _shortlistCookieService = shortlistCookieService;
        _sessionService = sessionService;
        _dateTimeService = dateTimeService;
        _courseIdValidator = courseIdValidator;
        _config = config.Value;
        _locationCookieService = locationCookieService;
    }

    [HttpPost]
    [Route("", Name = RouteNames.CourseProviders)]
    public IActionResult ApplyFilters(CourseProvidersFiltersSubmitModel submitModel)
    {
        var requestModel = new CourseProvidersFiltersRequestModel
        {
            LarsCode = submitModel.LarsCode,
            OrderBy = submitModel.OrderBy,
            DeliveryModes = submitModel.DeliveryModes,
            EmployerProviderRatings = submitModel.EmployerProviderRatings,
            ApprenticeProviderRatings = submitModel.ApprenticeProviderRatings,
            QarRatings = submitModel.QarRatings,
            PageNumber = 1
        };

        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location?.Trim(), Distance = submitModel.Distance });
        return RedirectToRoute(RouteNames.CourseProviders, requestModel);
    }

    [HttpGet]
    [Route("", Name = RouteNames.CourseProviders)]
    public async Task<IActionResult> CourseProviders(CourseProvidersFiltersRequestModel requestModel, bool clearFilter = false)
    {
        var validationLarsCodeResult = await _courseIdValidator.ValidateAsync(new GetCourseQuery { LarsCode = requestModel.LarsCode });

        if (!validationLarsCodeResult.IsValid)
        {
            return NotFound();
        }

        var (requestLocation, requestDistance) = _locationCookieService.GetLocation();
        if (clearFilter)
        {
            _locationCookieService.Delete(Constants.LocationCookieName);
            requestLocation = string.Empty;
            requestDistance = DistanceService.TenMiles.ToString();
        }

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);

        requestDistance = requestDistance ?? DistanceService.TenMiles.ToString();

        var orderBy = string.IsNullOrEmpty(requestLocation) && requestModel.OrderBy == ProviderOrderBy.Distance ? ProviderOrderBy.AchievementRate : requestModel.OrderBy;

        int? convertedDistance = DistanceService.GetValidDistanceNullable(requestDistance);

        var deliveryModes = requestModel.DeliveryModes.ToList();

        CourseProvidersViewModel CreateBaseCourseProvidersViewModel()
        {
            return new CourseProvidersViewModel(_config)
            {
                LarsCode = requestModel.LarsCode,
                ShortlistCount = shortlistCount?.Count ?? 0,
                OrderBy = orderBy,
                Location = requestLocation,
                Distance = convertedDistance.ToString(),
                SelectedDeliveryModes = deliveryModes.Select(d => d.ToString()),
                SelectedEmployerApprovalRatings = requestModel.EmployerProviderRatings.Select(r => r.ToString()),
                SelectedApprenticeApprovalRatings = requestModel.ApprenticeProviderRatings.Select(r => r.ToString()),
                SelectedQarRatings = requestModel.QarRatings.Select(q => q.ToString()),
                Providers = []
            };
        }

        var result = await _mediator.Send(new GetCourseProvidersQuery
        {
            LarsCode = requestModel.LarsCode,
            Location = requestLocation,
            OrderBy = orderBy,
            Distance = convertedDistance,
            DeliveryModes = deliveryModes.Count == 3 ? [] : deliveryModes,
            EmployerProviderRatings = requestModel.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatings = requestModel.ApprenticeProviderRatings.ToList(),
            Qar = requestModel.QarRatings.ToList(),
            Page = requestModel.PageNumber,
            ShortlistUserId = shortlistUserId
        });

        if (result == null)
        {
            return NotFound();
        }

        var courseProvidersViewModel = CreateBaseCourseProvidersViewModel();

        courseProvidersViewModel.CourseTitleAndLevel = result.StandardName;
        courseProvidersViewModel.CourseType = result.CourseType;
        courseProvidersViewModel.LearningType = result.LearningType;
        courseProvidersViewModel.IsActiveAvailable = result.IsActiveAvailable;
        courseProvidersViewModel.ShowSearchCrumb = true;
        courseProvidersViewModel.ShowShortListLink = true;
        courseProvidersViewModel.ShowApprenticeTrainingCoursesCrumb = true;
        courseProvidersViewModel.ShowApprenticeTrainingCourseCrumb = true;
        courseProvidersViewModel.QarPeriod = result.QarPeriod;
        courseProvidersViewModel.ReviewPeriod = result.ReviewPeriod;
        courseProvidersViewModel.TotalCount = result.TotalCount;

        var providers = result.Providers.Select(p => (CoursesProviderViewModel)p).ToList();
        foreach (var provider in providers)
        {
            provider.Distance = requestDistance;
            provider.Location = requestLocation;
        }

        courseProvidersViewModel.ProviderOrderOptions = GenerateProviderOrderDropdown(orderBy, string.IsNullOrEmpty(requestLocation));

        courseProvidersViewModel.Providers = providers;

        courseProvidersViewModel.Pagination = new PaginationViewModel(1, 0, Constants.DefaultPageSize,
            Url, RouteNames.CourseProviders, []);


        if (result.Providers.Count > 0)
        {
            var paginationQueryParams = courseProvidersViewModel.ToQueryString();
            courseProvidersViewModel.Pagination = new PaginationViewModel(
                result.Page,
                result.TotalCount,
                result.PageSize,
                Url,
                RouteNames.CourseProviders,
                paginationQueryParams
            );
        }

        return View(courseProvidersViewModel);
    }

    [HttpPost]
    [Route("{ukprn}", Name = RouteNames.CourseProviderDetails)]
    public async Task<IActionResult> ApplyLocation([FromForm] ProviderLocationSubmitModel submitModel, [FromRoute] string larsCode, [FromRoute] int ukprn)
    {
        var (_, requestDistance) = _locationCookieService.GetLocation();
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location?.Trim(), Distance = requestDistance });
        return RedirectToRoute(RouteNames.CourseProviderDetails, new { ukprn, larsCode });
    }

    [HttpGet]
    [Route("{ukprn}", Name = RouteNames.CourseProviderDetails)]
    public async Task<IActionResult> CourseProviderDetails([FromRoute] string larsCode, [FromRoute] int ukprn, bool clearLocation = false)
    {

        var validationUkprnResult = await _ukprnValidator.ValidateAsync(new GetCourseProviderDetailsQuery { Ukprn = ukprn });

        if (!validationUkprnResult.IsValid)
        {
            return NotFound();
        }

        var validationLarsCodeResult = await _courseIdValidator.ValidateAsync(new GetCourseQuery { LarsCode = larsCode });

        if (!validationLarsCodeResult.IsValid)
        {
            return NotFound();
        }

        var (requestLocation, requestDistance) = _locationCookieService.GetLocation();
        if (clearLocation)
        {
            _locationCookieService.Delete(Constants.LocationCookieName);
            requestLocation = string.Empty;
            requestDistance = DistanceService.TenMiles.ToString();
        }

        var validationLocationResult = await _courseLocationValidator.ValidateAsync(new GetCourseLocationQuery { Location = requestLocation });

        if (!validationLocationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationLocationResult.Errors);
            _locationCookieService.Delete(Constants.LocationCookieName);
            requestLocation = string.Empty;
            requestDistance = DistanceService.TenMiles.ToString();
        }

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);

        var query = new GetCourseProviderDetailsQuery
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            Location = requestLocation,
            Distance = string.IsNullOrWhiteSpace(requestLocation) ? null : DistanceService.DefaultDistance,
            ShortlistUserId = shortlistUserId
        };

        GetCourseProviderQueryResult result = await _mediator.Send(query);

        if (result is null)
        {
            return NotFound();
        }

        var viewModel = (CourseProviderViewModel)result;
        viewModel.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(result.AnnualEmployerFeedbackDetails,
            result.AnnualApprenticeFeedbackDetails, _dateTimeService.GetDateTime());
        viewModel.LarsCode = larsCode;
        viewModel.Location = requestLocation;
        viewModel.Distance = requestDistance ?? DistanceService.TenMiles.ToString();
        viewModel.ShortlistId = result.ShortlistId;
        viewModel.ShowApprenticeTrainingCourseProvidersCrumb = true;
        viewModel.ShowApprenticeTrainingCourseCrumb = true;
        viewModel.ShowApprenticeTrainingCoursesCrumb = true;
        viewModel.ShowShortListLink = true;
        viewModel.ShowSearchCrumb = true;
        viewModel.ShortlistCount = shortlistCount?.Count ?? 0;

        return View(viewModel);
    }
    private static List<ProviderOrderByOptionViewModel> GenerateProviderOrderDropdown(ProviderOrderBy orderBy, bool hideDistance)
    {
        var dropdown = new List<ProviderOrderByOptionViewModel>();

        foreach (ProviderOrderBy orderByChoice in Enum.GetValues<ProviderOrderBy>())
        {
            if (orderByChoice == ProviderOrderBy.Distance && hideDistance) continue;
            dropdown.Add(new ProviderOrderByOptionViewModel
            {
                ProviderOrderBy = orderByChoice,
                Description = orderByChoice.GetDescription(),
                Selected = orderByChoice == orderBy
            });
        }

        return dropdown;
    }
}
