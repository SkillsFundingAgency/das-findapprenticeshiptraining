using Microsoft.AspNetCore.Http;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Services;

public static class LocationCookieHelper
{
    public static (string Location, string Distance) GetLocation(
        ICookieStorageService<LocationCookieItem> cookieService,
        HttpRequest request,
        bool clearFilter)
    {
        var hasDeletedLocationCookie = false;

        if (clearFilter || (request?.Query?.ContainsKey(FilterService.ClearFilters) ?? false))
        {
            cookieService?.Delete(Constants.LocationCookieName);
            hasDeletedLocationCookie = true;
        }

        var locationCookieItem = cookieService?.Get(Constants.LocationCookieName);

        if (hasDeletedLocationCookie)
        {
            locationCookieItem = null;
        }

        return (locationCookieItem?.Location ?? string.Empty, locationCookieItem?.Distance ?? DistanceService.TenMiles.ToString());
    }
}
