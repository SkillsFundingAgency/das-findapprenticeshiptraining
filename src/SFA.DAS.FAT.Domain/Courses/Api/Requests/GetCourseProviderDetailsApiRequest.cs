using System;
using System.Web;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests;

public class GetCourseProviderDetailsApiRequest : IGetApiRequest
{
    private readonly string _larsCode;
    private readonly long _ukprn;
    private readonly string _location;
    private readonly int? _distance;
    private readonly Guid _shortlistUserId;

    public GetCourseProviderDetailsApiRequest(string baseUrl, string larsCode, long ukprn, string location, int? distance, Guid shortlistUserId)
    {
        _larsCode = larsCode;
        _ukprn = ukprn;
        _location = location;
        _distance = distance;
        _shortlistUserId = shortlistUserId;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }
    public string GetUrl => $"{BaseUrl}courses/{_larsCode}/providers/{_ukprn}?location={HttpUtility.UrlEncode(_location)}&distance={_distance}&shortlistUserId={_shortlistUserId}";
}
