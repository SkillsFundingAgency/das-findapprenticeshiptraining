using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public sealed class GetCourseRoutesApiRequest : IGetApiRequest
{
    public GetCourseRoutesApiRequest(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}courses/routes";
}
