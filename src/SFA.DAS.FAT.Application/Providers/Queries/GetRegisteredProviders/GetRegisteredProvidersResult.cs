using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Providers;

namespace SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;
public class GetRegisteredProvidersResult
{
    public IEnumerable<RegisteredProvider> Providers { get; set; }
}
