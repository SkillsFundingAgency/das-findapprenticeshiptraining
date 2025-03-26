using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("[controller]")]
public class ShortlistController : Controller
{
    public const int ShortlistExpiryInDays = 30;

    private readonly IMediator _mediator;
    private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
    private readonly ILogger<ShortlistController> _logger;
    private readonly IDataProtector _protector;

    public ShortlistController(IMediator mediator,
        ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
        IDataProtectionProvider provider,
        ILogger<ShortlistController> logger)
    {
        _mediator = mediator;
        _shortlistCookieService = shortlistCookieService;
        _logger = logger;
        _protector = provider.CreateProtector(Constants.ShortlistProtectorName);
    }

    [HttpGet]
    [Route("", Name = RouteNames.ShortList)]
    public async Task<IActionResult> Index([FromQuery] string removed)
    {
        var cookie = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        if (cookie == default)
        {
            return View(new ShortlistViewModel());
        }

        var result =
            await _mediator.Send(
                new GetShortlistForUserQuery { ShortlistUserId = cookie.ShortlistUserId });

        var removedProviderName = string.Empty;

        if (!string.IsNullOrEmpty(removed))
        {
            try
            {
                var base64EncodedBytes = WebEncoders.Base64UrlDecode(removed);
                removedProviderName = System.Text.Encoding.UTF8.GetString(_protector.Unprotect(base64EncodedBytes));
            }
            catch (FormatException e)
            {
                _logger.LogInformation(e, "Unable to decode provider name from request");
            }
            catch (CryptographicException e)
            {
                _logger.LogInformation(e, "Unable to decode provider name from request");
            }
        }

        var viewModel = new ShortlistViewModel
        {
            Shortlist = result.Shortlist.Select(item => (ShortlistItemViewModel)item).ToList(),
            Removed = removedProviderName
        };

        return View(viewModel);
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
            return RedirectToRoute(request.RouteName, new
            {
                Id = request.TrainingCode,
                ProviderId = request.Ukprn,
                Removed = string.IsNullOrEmpty(request.ProviderName) ? "" : WebEncoders.Base64UrlEncode(_protector.Protect(
                    System.Text.Encoding.UTF8.GetBytes($"{request.ProviderName}")))
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
