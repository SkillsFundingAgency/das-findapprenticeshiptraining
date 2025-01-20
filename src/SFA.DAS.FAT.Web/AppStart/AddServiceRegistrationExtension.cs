using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Application.Locations.Services;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Api;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddHttpClient<IApiClient, ApiClient>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IProviderService, ProviderService>();
        services.AddTransient<ILocationService, LocationService>();
        services.AddTransient<IShortlistService, ShortlistService>();
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddSingleton(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));
        services.AddHttpContextAccessor();
        AddDistributedCache(services, configuration);
    }

    private static void AddDistributedCache(IServiceCollection services, IConfigurationRoot configuration)
    {
        var localEnvs = new[] { "LOCAL", "DEV" };
        if (localEnvs.Contains(configuration["EnvironmentName"]))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var fatWebConfig = configuration.GetSection(nameof(FindApprenticeshipTrainingWeb)).Get<FindApprenticeshipTrainingWeb>();
                options.Configuration = fatWebConfig.RedisConnectionString;
            });
        }
    }
}
