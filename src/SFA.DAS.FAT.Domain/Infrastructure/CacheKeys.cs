using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.FAT.Domain.Infrastructure;

[ExcludeFromCodeCoverage]
public static class CacheKeys
{
    public const string RegisteredProviders = nameof(RegisteredProviders);
    public const string Levels = nameof(Levels);
    public const string Routes = nameof(Routes);
}

