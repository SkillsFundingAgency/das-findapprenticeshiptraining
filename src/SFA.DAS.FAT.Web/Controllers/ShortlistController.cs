using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;
using SFA.DAS.FAT.Domain;
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
public class ShortlistController : Controller
{
    public const int ShortlistExpiryInDays = 30;
    public const string RemovedProviderNameTempDataKey = "RemovedProviderName";
    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly IDataProtector _protector;
    private readonly IRequestApprenticeshipTrainingService _requestApprenticeshipTrainingService;
    private readonly ISessionService _sessionService;

    public ShortlistController(
        IMediator mediator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IDataProtectionProvider provider,
        IRequestApprenticeshipTrainingService requestApprenticeshipTrainingService,
        ISessionService sessionService)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _protector = provider.CreateProtector(Constants.ShortlistProtectorName);
        _requestApprenticeshipTrainingService = requestApprenticeshipTrainingService;
        _sessionService = sessionService;
    }

    [HttpGet]
    [Route("", Name = RouteNames.ShortLists)]
    public async Task<IActionResult> Index()
    {
        var cookie = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        if (cookie == default)
        {
            return View(new ShortlistsViewModel());
        }

        GetShortlistsForUserResponse result = await _mediator.Send(new GetShortlistsForUserQuery(cookie.ShortlistUserId));

        ShortlistsViewModel viewModel = GetShortlistsViewModel(result);

        viewModel.RemovedProviderName = TempData[RemovedProviderNameTempDataKey]?.ToString();

        var shortlistCount = _sessionService.Get<ShortlistsCount>();
        viewModel.HasMaxedOutShortlists = shortlistCount?.Count >= ShortlistConstants.MaximumShortlistCount;

        return View(viewModel);
    }

    private ShortlistsViewModel GetShortlistsViewModel(GetShortlistsForUserResponse result)
    {
        ShortlistsViewModel model = new();

        model.ExpiryDateText = result.ShortlistsExpiryDate.ToString("d MMMM yyyy");

        foreach (var course in result.Courses)
        {
            ShortlistCourseViewModel courseModel = new()
            {
                LarsCode = course.LarsCode,
                CourseTitle = course.StandardName
            };
            foreach (var location in course.Locations)
            {
                ShortlistLocationViewModel locationModel = new()
                {
                    Description = location.LocationDescription,
                    QarPeriod = $"20{result.QarPeriod.AsSpan(0, 2)} to 20{result.QarPeriod.AsSpan(2, 2)}",
                    ReviewPeriod = $"20{result.ReviewPeriod.AsSpan(0, 2)} to 20{result.ReviewPeriod.AsSpan(2, 2)}",
                    RequestApprenticeshipTraining = new()
                    {
                        CourseTitle = course.StandardName,
                        Url = _requestApprenticeshipTrainingService.GetRequestApprenticeshipTrainingUrl(course.LarsCode, EntryPoint.Shortlist, location.LocationDescription)
                    }
                };
                foreach (var provider in location.Providers)
                {
                    ShortlistProviderViewModel providerModel = new()
                    {
                        LarsCode = course.LarsCode,
                        LocationDescription = location.LocationDescription,
                        ShortlistId = provider.ShortlistId,
                        Ukprn = provider.Ukprn,
                        ProviderName = provider.ProviderName,
                        AtEmployer = provider.AtEmployer,
                        HasBlockRelease = provider.HasBlockRelease,
                        BlockReleaseDistance = provider.BlockReleaseDistance,
                        HasMultipleBlockRelease = provider.BlockReleaseCount > 1,
                        HasDayRelease = provider.HasDayRelease,
                        DayReleaseDistance = provider.DayReleaseDistance,
                        HasMultipleDayRelease = provider.DayReleaseCount > 1,
                        Email = provider.Email,
                        Phone = provider.Phone,
                        Website = provider.Website,
                        Leavers = provider.Leavers,
                        AchievementRate = provider.AchievementRate,
                        EmployerReviews = new() { ProviderRatingType = ProviderRatingType.Employer, Stars = provider.EmployerStars, Reviews = provider.EmployerReviews, ProviderRating = Enum.Parse<ProviderRating>(provider.EmployerRating) },
                        ApprenticeReviews = new() { ProviderRatingType = ProviderRatingType.Apprentice, Stars = provider.ApprenticeStars, Reviews = provider.ApprenticeReviews, ProviderRating = Enum.Parse<ProviderRating>(provider.ApprenticeRating) }
                    };

                    locationModel.Providers.Add(providerModel);
                }
                courseModel.Locations.Add(locationModel);
            }
            model.Courses.Add(courseModel);
        }

        return model;
    }

    [HttpPost]
    [Route("", Name = RouteNames.CreateShortlistItem)]
    public async Task<IActionResult> CreateShortlistItem(CreateShortlistItemRequest request)
    {
        var cookie = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        if (cookie == null)
        {
            cookie = new ShortlistCookieItem
            {
                ShortlistUserId = Guid.NewGuid()
            };
        }

        var result = await _mediator.Send(new CreateShortlistItemForUserCommand
        {
            Ukprn = request.Ukprn,
            LocationName = request.LocationName,
            LarsCode = request.LarsCode,
            ShortlistUserId = cookie.ShortlistUserId
        });

        _shortlistCookieService.Update(Constants.ShortlistCookieName, cookie, ShortlistExpiryInDays);

        if (!string.IsNullOrEmpty(request.RouteName))
        {
            return RedirectToRoute(request.RouteName, new
            {
                Id = request.LarsCode,
                ProviderId = request.Ukprn,
                Added = string.IsNullOrEmpty(request.ProviderName) ? "" : WebEncoders.Base64UrlEncode(_protector.Protect(
                    System.Text.Encoding.UTF8.GetBytes($"{request.ProviderName}")))
            });
        }

        return Accepted(result);
    }

    [HttpPost]
    [Route("items/{id}", Name = RouteNames.DeleteShortlistItem)]
    public async Task<IActionResult> DeleteShortlistItemForUser(DeleteShortlistItemRequest request)
    {
        await _mediator.Send(new DeleteShortlistItemForUserCommand
        {
            Id = request.ShortlistId,
        });

        if (!string.IsNullOrEmpty(request.RouteName))
        {
            TempData[RemovedProviderNameTempDataKey] = request.ProviderName;
            return RedirectToRoute(request.RouteName, new
            {
                Id = request.TrainingCode,
                ProviderId = request.Ukprn
            });
        }

        return Accepted();
    }

    /// <summary>
    /// This is a workaround to update the shortlist component via java script
    /// </summary>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    [HttpGet]
    [Route("UpdateCount")]
    public async Task<IActionResult> UpdateShortlistsCount()
    {
        return await Task.FromResult(ViewComponent("ShortlistsLink"));
    }
}
