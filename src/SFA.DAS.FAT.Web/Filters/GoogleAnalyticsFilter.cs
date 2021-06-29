using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        private readonly ICookieStorageService<LocationCookieItem> _locationCookieStorageService;
        private readonly ILogger<GoogleAnalyticsFilter> _logger;
        private readonly IDataProtector _protector;

        public GoogleAnalyticsFilter(ICookieStorageService<LocationCookieItem> locationCookieStorageService,
            IDataProtectionProvider provider, ILogger<GoogleAnalyticsFilter> logger)
        {
            _locationCookieStorageService = locationCookieStorageService;
            _logger = logger;
            _protector = provider.CreateProtector(Constants.GaDataProtectorName);
            
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var gaData = new GaData();
            var locationFromCookie = _locationCookieStorageService.Get(Constants.LocationCookieName);
            
            if (context.HttpContext.Request.Query.TryGetValue("location", out var location))
            {
                gaData.Location = location.ToString();
            }
            else if (locationFromCookie != null)
            {
                if (!string.IsNullOrEmpty(locationFromCookie.Name) && locationFromCookie.Lat != 0 &&
                    locationFromCookie.Lon != 0)
                {
                    gaData.Location = locationFromCookie.Name;
                }
            }
            
            
            if (context.RouteData.Values.TryGetValue("providerId", out var providerId))
            {
                if (uint.TryParse(providerId.ToString(), out var ukprn))
                {
                    if (context.HttpContext.Request.Query.TryGetValue("data", out var data))
                    {
                        try
                        {
                            var base64EncodedBytes = WebEncoders.Base64UrlDecode(data);
                            var decoded = System.Text.Encoding.UTF8.GetString(_protector.Unprotect(base64EncodedBytes));
                            var decodedItems = decoded.Split("|").ToList();
                            gaData.ProviderId = ukprn;
                            gaData.ProviderPlacement = Convert.ToInt32(decodedItems.FirstOrDefault());
                            gaData.ProviderTotal = Convert.ToInt32(decodedItems.LastOrDefault());
                        }
                        catch (FormatException)
                        {
                            _logger.LogInformation("Unable to decode GA data");
                        }
                        catch (CryptographicException)
                        {
                            _logger.LogInformation("Unable to unprotect GA data");
                        }
                        
                    }
                }
            }

            controller.ViewBag.GaData = gaData;

            base.OnActionExecuting(context);
        }
    }
}
