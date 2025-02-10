using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SFA.DAS.FAT.Application.CourseProviders.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers;
public class CoursesProvidersController : Controller
{
    private readonly ILogger<CoursesProvidersController> _logger;
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly IShortlistService _shortlistService;
    private readonly IDataProtector _providerDataProtector;

    public CoursesProvidersController(
        IMediator mediator,
        ILogger<CoursesProvidersController> logger,
        IDataProtectionProvider provider,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService, IShortlistService shortlistService)
    {
        _mediator = mediator;
        _logger = logger;
        _shortlistCookieService = shortlistCookieService;
        _shortlistService = shortlistService;
        _providerDataProtector = provider.CreateProtector(Constants.GaDataProtectorName);
    }


    [Route("courses/{id}/providers", Name = RouteNames.CourseProviders)]
    public async Task<IActionResult> CourseProviders(GetCourseProvidersRequest request)
    {
        // MFCMFC figure out shortlistUserId and shortlistItemCount in new architecture

        var shortListItemCount = 0;

        var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;

        if (shortlistUserId != null)
        {
            ShortlistForUser shortlist = await _shortlistService.GetShortlistForUser((Guid)shortlistUserId);
            shortListItemCount = shortlist.Shortlist.Count();
        }

        request.OrderBy = request.OrderBy ?? ProviderOrderBy.Distance;


        var result = await _mediator.Send(new GetCourseProvidersQuery
        {
            Id = request.Id,
            Location = request.Location,
            OrderBy = request.OrderBy,
            Distance = request.Distance,
            DeliveryModes = request.DeliveryModes.ToList(),
            EmployerProviderRatings = request.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatings = request.EmployerProviderRatings.ToList(),
            Qar = request.QarRatings.ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            ShortlistUserId = shortlistUserId
        });

        var providerList = result.Providers.ToList();

        var totalFiltered = result.TotalCount;


        var providers = providerList
                .ToDictionary(provider =>
                                (uint)provider.Ukprn,
                                provider => WebEncoders.Base64UrlEncode(_providerDataProtector.Protect(
                                    System.Text.Encoding.UTF8.GetBytes($"{providerList.IndexOf(provider) + 1}|{totalFiltered}"))));

        var courseProvidersViewModel = new CourseProvidersViewModel(request, result, providers);

        courseProvidersViewModel.ShowSearchCrumb = true;
        courseProvidersViewModel.ShowShortListLink = true;
        courseProvidersViewModel.ShortListItemCount = shortListItemCount;


        courseProvidersViewModel.ShowApprenticeTrainingCoursesCrumb = true;
        courseProvidersViewModel.ShowApprenticeTrainingCourseCrumb = true;


        // MFCMFC This will be addressed in a follow up story
        // var removedProviderFromShortlist =
        //     string.IsNullOrEmpty(request.Removed) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(request.Removed));
        // var addedProviderToShortlist =
        //     string.IsNullOrEmpty(request.Added) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(request.Added));
        //
        // courseProvidersViewModel.BannerUpdateMessage = GetProvidersBannerUpdateMessage(removedProviderFromShortlist, addedProviderToShortlist);

        return View(courseProvidersViewModel);
    }
}
