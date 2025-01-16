using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Web.AppStart;
using StackExchange.Redis;

namespace SFA.DAS.FAT.Web.AppStart;

public static class AddDataProtectionExtensions
{
    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {

        var fatWebConfig = configuration.GetSection(nameof(FindApprenticeshipTrainingWeb))
            .Get<FindApprenticeshipTrainingWeb>();

        if (fatWebConfig != null
            && !string.IsNullOrEmpty(fatWebConfig.DataProtectionKeysDatabase)
            && !string.IsNullOrEmpty(fatWebConfig.RedisConnectionString))
        {
            var redisConnectionString = fatWebConfig.RedisConnectionString;
            var dataProtectionKeysDatabase = fatWebConfig.DataProtectionKeysDatabase;

            var redis = ConnectionMultiplexer
                .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            services.AddDataProtection()
                .SetApplicationName("das-find-apprenticeship-training")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }
    }
}
