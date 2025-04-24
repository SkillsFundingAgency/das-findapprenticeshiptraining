using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;
using StructureMap.Query;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("courses/{id}/providers")]
public class CourseProvidersController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ISessionService _sessionService;
    private readonly FindApprenticeshipTrainingWeb _config;

    public CourseProvidersController(
        IMediator mediator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IOptions<FindApprenticeshipTrainingWeb> config,
        ISessionService sessionService)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _sessionService = sessionService;
        _config = config.Value;
    }

    [Route("", Name = RouteNames.CourseProviders)]
    public async Task<IActionResult> CourseProviders(CourseProvidersRequest request)
    {
        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;
        var shortlistCount = _sessionService.Get<ShortlistsCount>();
        var orderBy = string.IsNullOrEmpty(request.Location) && request.OrderBy == ProviderOrderBy.Distance ? ProviderOrderBy.AchievementRate : request.OrderBy;

        if (string.IsNullOrWhiteSpace(request.Distance) || !DistanceService.IsValidDistance(request.Distance))
        {
            request.Distance = DistanceService.TEN_MILES.ToString();
        }

        int? convertedDistance = DistanceService.GetValidDistanceNullable(request.Distance);

        var deliveryModes = request.DeliveryModes.ToList();

        var result = await _mediator.Send(new GetCourseProvidersQuery
        {
            Id = request.Id,
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

        var courseProvidersViewModel = new CourseProvidersViewModel(_config, Url)
        {
            Id = request.Id,
            ShortlistCount = shortlistCount?.Count ?? 0,
            OrderBy = orderBy,
            CourseTitleAndLevel = result.StandardName,
            CourseId = request.Id,
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
            Providers = new List<CoursesProviderViewModel>()
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
