using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Extensions;

public static class LocationCookieExtention
{
    public static (string Location, string Distance) GetLocation(
        this ICookieStorageService<LocationCookieItem> cookieService)
    {
        var locationCookieItem = cookieService.Get(Constants.LocationCookieName);

        return (locationCookieItem?.Location ?? string.Empty, locationCookieItem?.Distance ?? DistanceService.DefaultDistance.ToString());
    }
}
