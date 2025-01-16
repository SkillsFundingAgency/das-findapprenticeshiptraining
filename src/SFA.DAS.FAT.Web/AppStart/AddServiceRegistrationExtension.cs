using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Application.Locations.Services;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Api;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddHttpClient<IApiClient, ApiClient>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<ILocationService, LocationService>();
        services.AddTransient<IShortlistService, ShortlistService>();
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddTransient(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));
    }
}
