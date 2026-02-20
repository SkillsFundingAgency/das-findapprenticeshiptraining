using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
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
    public const string LocationTempDataKey = "Location";
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
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
        IValidator<GetCourseQuery> courseIdValidator)
    {
        _mediator = mediator;
        _ukprnValidator = ukprnValidator;
        _courseLocationValidator = courseLocationValidator;
        _shortlistCookieService = shortlistCookieService;
        _sessionService = sessionService;
        _dateTimeService = dateTimeService;
        _courseIdValidator = courseIdValidator;
        _config = config.Value;
    }

    [Route("", Name = RouteNames.CourseProviders)]
    public async Task<IActionResult> CourseProviders(CourseProvidersRequest request)
    {
        var validationLarsCodeResult = await _courseIdValidator.ValidateAsync(new GetCourseQuery { LarsCode = request.LarsCode });

        if (!validationLarsCodeResult.IsValid)
        {
            return NotFound();
        }

        string prevLocation = TempData[LocationTempDataKey]?.ToString();
        TempData[LocationTempDataKey] = request.Location;

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);
        var orderBy = string.IsNullOrEmpty(request.Location) && request.OrderBy == ProviderOrderBy.Distance ? ProviderOrderBy.AchievementRate : request.OrderBy;

        if (string.IsNullOrEmpty(prevLocation) && !string.IsNullOrEmpty(request.Location))
        {
            orderBy = ProviderOrderBy.Distance;
        }

        if (string.IsNullOrWhiteSpace(request.Distance) || !DistanceService.IsValidDistance(request.Distance))
        {
            request.Distance = DistanceService.TEN_MILES.ToString();
        }

        int? convertedDistance = DistanceService.GetValidDistanceNullable(request.Distance);

        var deliveryModes = request.DeliveryModes.ToList();

        var result = await _mediator.Send(new GetCourseProvidersQuery
        {
            LarsCode = request.LarsCode,
            Location = request.Location,
            OrderBy = orderBy,
            Distance = convertedDistance,
            DeliveryModes = deliveryModes,
            EmployerProviderRatings = request.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatings = request.ApprenticeProviderRatings.ToList(),
            Qar = request.QarRatings.ToList(),
            Page = request.PageNumber,
            ShortlistUserId = shortlistUserId
        });

        if (result == null)
        {
            return NotFound();
        }

        var courseProvidersViewModel = new CourseProvidersViewModel(_config)
        {
            LarsCode = request.LarsCode,
            ShortlistCount = shortlistCount?.Count ?? 0,
            OrderBy = orderBy,
            CourseTitleAndLevel = result.StandardName,
            CourseType = result.CourseType,
            ApprenticeshipType = result.ApprenticeshipType,
            Location = request.Location,
            Distance = convertedDistance.ToString(),
            SelectedDeliveryModes = deliveryModes.Select(d => d.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(r => r.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(r => r.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(q => q.ToString()).ToList(),
            ShowSearchCrumb = true,
            ShowShortListLink = true,
            ShowApprenticeTrainingCoursesCrumb = true,
            ShowApprenticeTrainingCourseCrumb = true,
            QarPeriod = result.QarPeriod,
            ReviewPeriod = result.ReviewPeriod,
            TotalCount = result.TotalCount,
            Providers = []
        };

        var providers = result.Providers.Select(p => (CoursesProviderViewModel)p).ToList();
        foreach (var provider in providers)
        {
            provider.Distance = request.Distance;
            provider.Location = request.Location;
        }

        courseProvidersViewModel.ProviderOrderOptions = GenerateProviderOrderDropdown(orderBy, string.IsNullOrEmpty(request.Location));

        courseProvidersViewModel.Providers = providers;

        courseProvidersViewModel.Pagination = new PaginationViewModel(1, 0, Constants.DefaultPageSize,
            Url, RouteNames.CourseProviders, new List<ValueTuple<string, string>>());


        if (result.Providers.Count > 0)
        {
            courseProvidersViewModel.Pagination = new PaginationViewModel(
                result.Page,
                result.TotalCount,
                result.PageSize,
                Url,
                RouteNames.CourseProviders,
                courseProvidersViewModel.ToQueryString()
            );
        }

        return View(courseProvidersViewModel);
    }

    [Route("{providerId}", Name = RouteNames.CourseProviderDetails)]
    public async Task<IActionResult> CourseProviderDetails([FromRoute] string larsCode, [FromRoute] int providerId, [FromQuery] string location, [FromQuery] string distance)
    {

        var validationUkprnResult = await _ukprnValidator.ValidateAsync(new GetCourseProviderDetailsQuery { Ukprn = providerId });

        if (!validationUkprnResult.IsValid)
        {
            return NotFound();
        }

        var validationLarsCodeResult = await _courseIdValidator.ValidateAsync(new GetCourseQuery { LarsCode = larsCode });

        if (!validationLarsCodeResult.IsValid)
        {
            return NotFound();
        }

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>(SessionKeys.ShortlistCount);

        if (!string.IsNullOrWhiteSpace(location) && !DistanceService.IsValidDistance(distance))
        {
            distance = DistanceService.TEN_MILES.ToString();
        }

        var query = new GetCourseProviderDetailsQuery
        {
            Ukprn = providerId,
            LarsCode = larsCode,
            Location = location?.Trim(),
            Distance = string.IsNullOrWhiteSpace(location) ? null : DistanceService.DEFAULT_DISTANCE,
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
        viewModel.Location = location;
        viewModel.Distance = distance ?? DistanceService.TEN_MILES.ToString();
        viewModel.ShortlistId = result.ShortlistId;
        viewModel.ShowApprenticeTrainingCourseProvidersCrumb = true;
        viewModel.ShowApprenticeTrainingCourseCrumb = true;
        viewModel.ShowApprenticeTrainingCoursesCrumb = true;
        viewModel.ShowShortListLink = true;
        viewModel.ShowSearchCrumb = true;
        viewModel.ShortlistCount = shortlistCount?.Count ?? 0;

        var validationLocationResult = await _courseLocationValidator.ValidateAsync(new GetCourseLocationQuery { Location = location?.Trim() });

        if (!validationLocationResult.IsValid)
        {
            viewModel.Location = string.Empty;
            validationLocationResult.AddToModelState(ModelState);
        }

        return View(viewModel);
    }
    private static List<ProviderOrderByOptionViewModel> GenerateProviderOrderDropdown(ProviderOrderBy orderBy, bool hideDistance)
    {
        var dropdown = new List<ProviderOrderByOptionViewModel>();

        foreach (ProviderOrderBy orderByChoice in Enum.GetValues(typeof(ProviderOrderBy)))
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
