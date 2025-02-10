using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Domain.AcademicYears.Api;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.Services;
public class AcademicYearService : IAcademicYearsService
{
    private readonly IApiClient _apiClient;
    private readonly FindApprenticeshipTrainingApi _config;

    public AcademicYearService(IApiClient apiClient, IOptions<FindApprenticeshipTrainingApi> config)
    {
        _apiClient = apiClient;
        _config = config.Value;
    }

    public async Task<AcademicYearsLatest> GetAcademicYearsLatest()
    {
        //MFCMFC caching needed
        var request = new GetAcademicYearsLatestRequest(_config.BaseUrl);

        var response = await _apiClient.Get<AcademicYearsLatest>(request);

        return response;
    }
}
