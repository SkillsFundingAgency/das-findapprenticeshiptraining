using System;

namespace SFA.DAS.FAT.Domain.Infrastructure;

public record CacheInfo(string Key, TimeSpan CacheDuration);
