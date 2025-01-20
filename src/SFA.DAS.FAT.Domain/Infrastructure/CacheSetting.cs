using System;

namespace SFA.DAS.FAT.Domain.Infrastructure;

public static class CacheSetting
{
    public static CacheInfo Providers => new(CacheKeys.RegisteredProviders, TimeSpan.FromHours(1));
}
