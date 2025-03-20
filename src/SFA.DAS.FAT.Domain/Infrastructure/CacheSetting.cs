using System;

namespace SFA.DAS.FAT.Domain.Infrastructure;

public static class CacheSetting
{
    public static CacheInfo Providers => new(CacheKeys.RegisteredProviders, TimeSpan.FromHours(1));
    public static CacheInfo Levels => new(CacheKeys.Levels, TimeSpan.FromHours(24));
    public static CacheInfo Routes => new(CacheKeys.Routes, TimeSpan.FromHours(24));
    public static CacheInfo AcademicYearsLatest => new(CacheKeys.AcademicYearsLatest, TimeSpan.FromHours(24));
}
