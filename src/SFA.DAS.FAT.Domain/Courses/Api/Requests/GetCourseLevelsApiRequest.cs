using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public sealed class GetCourseLevelsApiRequest(string baseUrl) : IGetApiRequest
{
    public string BaseUrl { get; } = baseUrl;
    public string GetUrl => $"{BaseUrl}courses/levels";
}
