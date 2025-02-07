using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IRoutesService
{
    Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken);
}
