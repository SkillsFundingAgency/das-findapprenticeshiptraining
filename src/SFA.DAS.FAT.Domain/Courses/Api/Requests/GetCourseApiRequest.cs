using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public class GetCourseApiRequest : IGetApiRequest
{
    private readonly string _location;
    private readonly int? _distance;
    private readonly int _larsCode;

    public GetCourseApiRequest(string baseUrl, int larsCode, string location, int? distance)
    {
        _location = location;
        _distance = distance;
        _larsCode = larsCode;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }

    public string GetUrl => $"{BaseUrl}courses/{_larsCode}?location={_location}&distance={_distance}";
}
