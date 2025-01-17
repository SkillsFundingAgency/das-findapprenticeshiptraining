using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddSessionExtension
{
    public static IServiceCollection AddSession(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton<IDistributedCacheService, IDistributedCacheService>();
        AddDistributedCache(services, configuration);
        return services;
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
