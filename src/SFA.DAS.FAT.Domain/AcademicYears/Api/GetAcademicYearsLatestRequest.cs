using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.AcademicYears.Api;

public class GetAcademicYearsLatestRequest : IGetApiRequest
{

    public GetAcademicYearsLatestRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}academicYears/latest";
}
