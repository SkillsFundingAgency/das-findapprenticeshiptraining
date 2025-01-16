using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.FAT.Domain.Providers;

[ExcludeFromCodeCoverage]
public class RegisteredProvider
{
    public string Name { get; set; } = null!;
    public int Ukprn { get; set; }
}
