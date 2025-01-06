using System;
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

namespace SFA.DAS.FAT.Web.Controllers
{
    [Route("[controller]")]
    public class ShortlistController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
        private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;
        private readonly ILogger<ShortlistController> _logger;
        private readonly IDataProtector _protector;

        public ShortlistController(IMediator mediator,
            ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
            ICookieStorageService<LocationCookieItem> locationCookieService,
            IDataProtectionProvider provider,
            ILogger<ShortlistController> logger)
        {
            _mediator = mediator;
            _shortlistCookieService = shortlistCookieService;
            _locationCookieService = locationCookieService;
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
        [Route("courses/{id}/providers/{providerId}", Name = RouteNames.CreateShortlistItem)]
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
            _shortlistCookieService.Update(Constants.ShortlistCookieName, cookie, 30);

            var location = _locationCookieService.Get(Constants.LocationCookieName);

            var result = await _mediator.Send(new CreateShortlistItemForUserCommand
            {
                Lat = location?.Lat,
                Lon = location?.Lon,
                Ukprn = request.Ukprn,
                LocationDescription = string.IsNullOrEmpty(location?.Name) ? null : location.Name,
                TrainingCode = request.TrainingCode,
                ShortlistUserId = cookie.ShortlistUserId
            });

            if (!string.IsNullOrEmpty(request.RouteName))
            {
                return RedirectToRoute(request.RouteName, new
                {
                    Id = request.TrainingCode,
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
            var cookie = _shortlistCookieService.Get(Constants.ShortlistCookieName);
            if (cookie != null)
            {
                await _mediator.Send(new DeleteShortlistItemForUserCommand
                {
                    Id = request.ShortlistId,
                    ShortlistUserId = cookie.ShortlistUserId
                });
            }
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
    }
}
