using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface ILevelsService
{
    Task<IEnumerable<Level>> GetLevelsAsync(CancellationToken cancellationToken);
}
