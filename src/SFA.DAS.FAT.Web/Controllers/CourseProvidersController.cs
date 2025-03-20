using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;
public class CourseProvidersController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly FindApprenticeshipTrainingWeb _config;

    public CourseProvidersController(
        IMediator mediator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService, IOptions<FindApprenticeshipTrainingWeb> config)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _config = config.Value;
    }


    [Route("courses/{id}/providers", Name = RouteNames.CourseProviders)]
    public async Task<IActionResult> CourseProviders(int id, CourseProvidersRequest request)
    {
        request.Id = id;
        var shortListItemCount = 0;

        if (!string.IsNullOrWhiteSpace(request.Location) && (string.IsNullOrWhiteSpace(request.Distance) || !DistanceService.IsValidDistance(request.Distance)))
        {
            request.Distance = Constants.DefaultDistance.ToString();
        }

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;

        int? distanceToRequest = null;
        if (request.Distance != DistanceService.ACROSS_ENGLAND_FILTER_VALUE && int.TryParse(request.Distance, out var actualDistance))
        {
            distanceToRequest = actualDistance;
        }

        var result = await _mediator.Send(new GetCourseProvidersQuery
        {
            Id = request.Id,
            Location = request.Location,
            OrderBy = request.OrderBy,
            Distance = distanceToRequest,
            DeliveryModes = request.DeliveryModes.ToList(),
            EmployerProviderRatings = request.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatings = request.ApprenticeProviderRatings.ToList(),
            Qar = request.QarRatings.ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            ShortlistUserId = shortlistUserId
        });

        var courseProvidersViewModel = new CourseProvidersViewModel(_config, Url)
        {
            CourseTitleAndLevel = result.StandardName,
            CourseId = request.Id,
            Location = request.Location,
            Distance = distanceToRequest.ToString(),
            SelectedDeliveryModes = request.DeliveryModes.Where(dm => dm != ProviderDeliveryMode.Provider)
                .Select(d => d.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(r => r.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(r => r.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(q => q.ToString()).ToList(),
            ShowSearchCrumb = true,
            ShowShortListLink = true,
            ShortListItemCount = shortListItemCount,
            ShowApprenticeTrainingCoursesCrumb = true,
            ShowApprenticeTrainingCourseCrumb = true,
            QarPeriod = result.QarPeriod,
            ReviewPeriod = result.ReviewPeriod
        };

        return View(courseProvidersViewModel);
    }
}
