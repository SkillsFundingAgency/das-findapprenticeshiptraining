using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public sealed class GetCourseLevelsApiRequest : IGetApiRequest
{
    public GetCourseLevelsApiRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}courses/levels";
}
