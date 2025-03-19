using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Responses;

namespace SFA.DAS.FAT.Domain.Interfaces;
public interface IAcademicYearsService
{
    Task<GetAcademicYearsLatestResponse> GetAcademicYearsLatestAsync(CancellationToken cancellationToken);
}
