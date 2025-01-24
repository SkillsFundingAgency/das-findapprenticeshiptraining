using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Providers;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IProviderService
{
    Task<List<RegisteredProvider>> GetRegisteredProviders();
}
