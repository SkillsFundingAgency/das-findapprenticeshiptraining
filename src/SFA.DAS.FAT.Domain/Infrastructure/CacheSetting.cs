using System;

namespace SFA.DAS.FAT.Domain.Infrastructure;

public static class CacheSetting
{
    public static CacheInfo Providers => new(CacheKeys.RegisteredProviders, TimeSpan.FromHours(1));
    public static CacheInfo Levels => new("CourseLevels", TimeSpan.FromHours(24));
    public static CacheInfo Routes => new("CourseRoutes", TimeSpan.FromHours(24));
    public static CacheInfo AcademicYearsLatest => new(CacheKeys.AcademicYearsLatest, TimeSpan.FromHours(24));
}
